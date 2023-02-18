
mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';

window.onload = (event) => {
    let userCoordinates;
    navigator.geolocation.getCurrentPosition(function (position) {
        userCoordinates = position;
        $.ajax({
            type: 'GET',
            url: '/Clubs/CoordsMarkers'
        }).done(function (response) {
            if (userCoordinates) {
                loadMap(userCoordinates, response);
            } else {
                loadMap(response);
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            console.log("Erro: " + textStatus + ", " + errorThrown);
            console.log("Resposta do servidor: " + jqXHR.responseText);
        });
    });

   
};

var map;

function flyToClub(coordX,coordY) {
    if (coordX != '' && coordY != '') {
        map.flyTo({ center: [coordX, coordY], zoom: 14 });
    }   
}

function loadMap(userCoordinates, markersCoordinates) {
    map = new mapboxgl.Map({
        container: 'map', // container ID
        style: 'mapbox://styles/mapbox/streets-v12',
        zoom: 10,
        center: [-8.8926, 38.5243]
    });

    const geocoder = new MapboxGeocoder({
        accessToken: mapboxgl.accessToken,
        mapboxgl: mapboxgl,
        marker: false,
        placeholder: 'Digite um endereço ou localização'
    });

    if (userCoordinates) {
        // Define o centro do mapa para a posição do utilizador
        map.setCenter([userCoordinates.coords.longitude, userCoordinates.coords.latitude]);
    }

    map.on('load', () => {
        map.resize();
        var coords = markersCoordinates;
        Object.keys(coords).forEach(function (key) {
            new mapboxgl.Marker()
                .setLngLat([coords[key].coordinateX, coords[key].coordinateY])
                .addTo(map)
                .setPopup(new mapboxgl.Popup().setHTML('<h1>Clube ' + coords[key].name + '</h1>'))
                .on('click', function () {
                    map.flyTo({
                        center: [coords[key].coordinateX, coords[key].coordinateY],
                        zoom: 14
                    });
                });

        });

    });

    map.addControl(geocoder, 'top-left');
   
}