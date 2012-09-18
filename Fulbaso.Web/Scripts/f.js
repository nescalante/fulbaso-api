var map;

var f = f || {
    map: {
        initialize: function (id, lat, lng) {
            if (!map) {
                var options = { zoom: 15, mapTypeId: google.maps.MapTypeId.ROADMAP }
                map = new google.maps.Map(document.getElementById(id), options);
                if (lat && lng) {
                    map.setCenter(new google.maps.LatLng(lat, lng));
                }
                else {
                    map.setCenter(new google.maps.LatLng(-38, -63));
                    map.setZoom(4);
                }
            }

            map.id = id;
            map.hover = new Array();
            map.close = new Array();
            map.geocoder = new google.maps.Geocoder();

            return map;
        },
        openWindow: function (infowindow, marker) {
            infowindow.open(map, marker);

            if (map.lastWindow && map.lastWindow != infowindow) {
                map.lastWindow.close();
            }

            map.lastWindow = infowindow;
        },
        getWindow: function (place, marker) {
            var html = "<div class=\"infoview\"><a href=\"" + place.Url + "\"><h4>" + place.Description + "</h4></a>";
            html += "<ul>";
            html += "<li class=\"ui-info address\">" + place.Address + "</li>";
            if (place.Phone && place.Phone != "") {
                html += "<li class=\"ui-info phone\">" + place.Phone + "</li>";
            }
            html += "<li class=\"ui-info courts\">" + place.Courts + (place.Courts == 1 ? " cancha" : " canchas") + "</li>";
            html += "</ul></div>";

            var infowindow = new google.maps.InfoWindow({
                content: html
            });

            google.maps.event.addListener(marker, 'click', function () {
                f.map.openWindow(infowindow, marker);
            });

            google.maps.event.addListener(map, 'click', function () {
                infowindow.close();
                map.lastWindow = undefined;
            });

            return infowindow;
        },
        mark: function (title, lat, lng, icon, place) {
            var marker;

            if (icon) {
                marker = new google.maps.Marker({ map: map, title: title, icon: new google.maps.MarkerImage(icon), position: new google.maps.LatLng(lat, lng) });
            }
            else {
                marker = new google.maps.Marker({ map: map, title: title, position: new google.maps.LatLng(lat, lng) });
            }

            if (place) {
                var container = $("#" + map.id).parent();
                var infowindow = f.map.getWindow(place, marker);

                map.hover[place.Id] = function () {
                    console.log(this);
                    f.map.setCenter(place.MapUa, place.MapVa);
                    f.map.openWindow(infowindow, marker);
                    return false;
                };
                map.close[place.Id] = function () {
                    infowindow.close();
                }

                container.find(".mapitem[data-id=" + place.Id + "]").hover(map.hover[place.Id], map.close[place.Id]);
            }

            return marker;
        },
        markFrom: function (service, icon, reference) {
            $.getJSON(service, function (data) {
                if (reference) {
                    $.get(reference, function (html) {
                        var container = $("#" + map.id).parent();
                        $(html).hide().appendTo(container).fadeIn();

                        $.each(data, function () {
                            var self = this.Item1 ? this.Item1 : this;
                            f.map.mark(self.Description, self.MapUa, self.MapVa, icon, self);
                        });
                    });
                }
                else {
                    $.each(data, function () {
                        var self = this.Item1 ? this.Item1 : this;
                        f.map.mark(self.Description, self.MapUa, self.MapVa, icon, self);
                    });
                }
            });
        },
        setCenter: function (lat, lng) {
            map.setCenter(new google.maps.LatLng(lat, lng));
        }
    },
    location: {
        initialize: function (id, successfunction) {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(success);
            }

            function success(position) {
                f.map.initialize(id, position.coords.latitude, position.coords.longitude);
                $("#" + id).fadeIn();

                if (successfunction) {
                    successfunction(position);
                }
            }
        }
    },
    places: {
        load: function (id, reference) {
            $.get(reference, function (html) {
                $("#" + id).parent().fadeIn();
                var container = $("#" + id);
                $(html).hide().appendTo(container).fadeIn();
                $(".mapitem").hover(function () { map.hover[$(this).data("id")]() }, function () { map.close[$(this).data("id")]() });
            });
        }
    },
    geocode: {
        find: function (location, description, jsonContainer, onchange) {
            map.geocoder.geocode({ address: location }, function (results, status) {
                if (status == google.maps.GeocoderStatus.OK) {
                    var latlng = results[0].geometry.location;
                    var lat = latlng.lat();
                    var lng = latlng.lng();

                    if (map.marker) {
                        map.marker.setMap(null);
                    }
                    if (jsonContainer) {
                        jsonContainer.val(JSON.stringify(results[0].address_components));
                    }

                    map.marker = f.map.mark(description, lat, lng);
                    map.marker.setDraggable(true);
                    map.setCenter(latlng);
                    map.setZoom(15);

                    if (onchange) {
                        google.maps.event.addListener(map.marker, 'position_changed', onchange);
                    }
                }
            });
        }
    }
};