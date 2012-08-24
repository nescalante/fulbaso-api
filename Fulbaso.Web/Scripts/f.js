var map;

var f = f || {
    map: {
        initialize: function (id, lat, lng, infoservice) {
            var latlng = new google.maps.LatLng(lat, lng);
            var options = { zoom: 15, center: latlng, mapTypeId: google.maps.MapTypeId.ROADMAP }
            map = new google.maps.Map(document.getElementById(id), options);
            map.id = id;
            map.infoservice = infoservice;
            map.setCenter(latlng);

            return map;
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
                        infowindow.open(map, marker);
                    });

                    if (place) {
                        container.find(".mapitem[data-id=" + place.Id + "]").hover(function () {
                            f.map.setCenter($(this).data("ua"), $(this).data("va"));
                            infowindow.open(map, marker);
                            return false;
                        }, function () { infowindow.close(map, marker); })
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
    }
};