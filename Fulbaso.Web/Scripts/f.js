var map;

var f = f || {
    map: {
        initialize: function (id, lat, lng, infoservice) {
            if (!map) {
                var latlng = new google.maps.LatLng(lat, lng);
                var options = { zoom: 15, center: latlng, mapTypeId: google.maps.MapTypeId.ROADMAP }
                map = new google.maps.Map(document.getElementById(id), options);
                map.setCenter(latlng);
            }

            map.id = id;
            map.infoservice = infoservice;

            return map;
        },
        openWindow: function (infowindow, marker) {
            infowindow.open(map, marker);

            if (map.lastWindow && map.lastWindow != infowindow) {
                console.log("closed");
                map.lastWindow.close();
            }

            map.lastWindow = infowindow;
        },
        mark: function (title, lat, lng, icon, place) {
            var marker;

            if (icon) {
                marker = new google.maps.Marker({ map: map, title: title, icon: new google.maps.MarkerImage(icon), position: new google.maps.LatLng(lat, lng) });
            }
            else {
                marker = new google.maps.Marker({ map: map, title: title, position: new google.maps.LatLng(lat, lng) });
            }

            if (map.infoservice && place) {
                var container = $("#" + map.id).parent();

                $.get(map.infoservice, place, function (html) {
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

                    if (place) {
                        container.find(".mapitem[data-id=" + place.Id + "]").hover(function () {
                            f.map.setCenter($(this).data("ua"), $(this).data("va"));
                            f.map.openWindow(infowindow, marker);
                            return false;
                        }, function () { infowindow.close(); })
                    }
                });
            }
        },
        markFrom: function (service, icon, reference) {
            $.getJSON(service, function (data) {
                if (reference) {
                    $.get(reference, function (html) {
                        var container = $("#" + map.id).parent();
                        $(html).hide().appendTo(container).fadeIn();
                    });
                }

                $.each(data, function () {
                    f.map.mark(this.Item1.Description, this.Item1.MapUa, this.Item1.MapVa, icon, this.Item1);
                });
            });
        },
        setCenter: function (lat, lng) {
            map.setCenter(new google.maps.LatLng(lat, lng));
        }
    },
    location: {
        initialize: function (id, infoservice, successfunction) {
            if (navigator.geolocation) {
                navigator.geolocation.getCurrentPosition(success);
            }

            function success(position) {
                f.map.initialize(id, position.coords.latitude, position.coords.longitude, infoservice);
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
            });
        }
    }
};