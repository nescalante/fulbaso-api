﻿var map;var f=f||{map:{initialize:function(a,b,c){if(!map){var d={zoom:15,mapTypeId:google.maps.MapTypeId.ROADMAP};map=new google.maps.Map(document.getElementById(a),d);if(b&&c){map.setCenter(new google.maps.LatLng(b,c))}else{map.setCenter(new google.maps.LatLng(-38,-63));map.setZoom(4)}}map.id=a;map.hover=new Array();map.close=new Array();map.geocoder=new google.maps.Geocoder();return map},openWindow:function(a,b){a.open(map,b);if(map.lastWindow&&map.lastWindow!=a){map.lastWindow.close()}map.lastWindow=a},getWindow:function(d,c){var a='<div class="infoview"><a href="'+d.Url+'"><h4>'+d.Description+"</h4></a>";a+="<ul>";a+='<li class="ui-info address">'+d.Address+"</li>";if(d.Phone&&d.Phone!=""){a+='<li class="ui-info phone">'+d.Phone+"</li>"}a+='<li class="ui-info courts">'+d.Courts+(d.Courts==1?" cancha":" canchas")+"</li>";a+="</ul></div>";var b=new google.maps.InfoWindow({content:a});google.maps.event.addListener(c,"click",function(){f.map.openWindow(b,c)});google.maps.event.addListener(map,"click",function(){b.close();map.lastWindow=undefined});return b},mark:function(i,d,e,b,h){var g;if(b){g=new google.maps.Marker({map:map,title:i,icon:new google.maps.MarkerImage(b),position:new google.maps.LatLng(d,e)})}else{g=new google.maps.Marker({map:map,title:i,position:new google.maps.LatLng(d,e)})}if(h){var a=$("#"+map.id).parent();var c=f.map.getWindow(h,g);map.hover[h.Id]=function(){console.log(this);f.map.setCenter(h.MapUa,h.MapVa);f.map.openWindow(c,g);return false};map.close[h.Id]=function(){c.close()};a.find(".mapitem[data-id="+h.Id+"]").hover(map.hover[h.Id],map.close[h.Id])}return g},markFrom:function(c,a,b){$.getJSON(c,function(d){if(b){$.get(b,function(g){var e=$("#"+map.id).parent();$(g).hide().appendTo(e).fadeIn();$.each(d,function(){var h=this.Item1?this.Item1:this;f.map.mark(h.Description,h.MapUa,h.MapVa,a,h)})})}else{$.each(d,function(){var e=this.Item1?this.Item1:this;f.map.mark(e.Description,e.MapUa,e.MapVa,a,e)})}})},setCenter:function(a,b){map.setCenter(new google.maps.LatLng(a,b))}},location:{initialize:function(a,c){if(navigator.geolocation){navigator.geolocation.getCurrentPosition(b)}function b(d){f.map.initialize(a,d.coords.latitude,d.coords.longitude);$("#"+a).fadeIn();if(c){c(d)}}}},places:{load:function(a,b){$.get(b,function(d){$("#"+a).parent().fadeIn();var c=$("#"+a);$(d).hide().appendTo(c).fadeIn();$(".mapitem").hover(function(){map.hover[$(this).data("id")]()},function(){map.close[$(this).data("id")]()})})}},geocode:{find:function(c,a,b,d){map.geocoder.geocode({address:c},function(i,j){if(j==google.maps.GeocoderStatus.OK){var g=i[0].geometry.location;var e=g.lat();var h=g.lng();if(map.marker){map.marker.setMap(null)}if(b){b.val(JSON.stringify(i[0].address_components))}map.marker=f.map.mark(a,e,h);map.marker.setDraggable(true);map.setCenter(g);map.setZoom(15);if(d){google.maps.event.addListener(map.marker,"position_changed",d)}d(i[0])}})}}};