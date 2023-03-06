
mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';

window.onload = (event) => {
    let defaultCoords = { coords: { longitude: -8.896442, latitude: 38.533278 } };

    navigator.geolocation.getCurrentPosition(function (position) {
        let userCoordinates = position;
        getMarkers(userCoordinates);
    }, function () {
        getMarkers(defaultCoords);
    });
};

function getMarkers(coords) {
    $.ajax({
        type: 'GET',
        url: '/Clubs/CoordsMarkers'
    }).done(function (response) {
        loadMap(coords, response);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log("Erro: " + textStatus + ", " + errorThrown);
        console.log("Resposta do servidor: " + jqXHR.responseText);
    });
}

var map;

function flyToClub(coordX, coordY) {
    if (coordX != '' && coordY != '') {
        map.flyTo({ center: [parseFloat((coordX).replace(',', '.')), parseFloat((coordY).replace(',', '.'))], zoom: 14 });
    }
}

function loadMap(userCoordinates, markersCoordinates) {
    map = new mapboxgl.Map({
        container: 'map', // container ID
        style: 'mapbox://styles/mapbox/streets-v12',
        zoom: 10,
        center: [userCoordinates.coords.longitude, userCoordinates.coords.latitude]
    });

    const layerList = document.getElementById('menu');
    const inputs = layerList.getElementsByTagName('input');

    for (const input of inputs) {
        input.onclick = (layer) => {
            const layerId = layer.target.id;
            map.setStyle('mapbox://styles/mapbox/' + layerId);
        };
    }

    const geocoder = new MapboxGeocoder({
        accessToken: mapboxgl.accessToken,
        mapboxgl: mapboxgl,
        marker: false,
        placeholder: 'Digite um endereço ou localização'
    });

    map.on('load', () => {
        map.resize();
        let coords = markersCoordinates;
        Object.keys(coords).forEach(function (key) {
            new mapboxgl.Marker({ color: "#00639A"})
                .setLngLat([coords[key].coordinateX, coords[key].coordinateY])
                .addTo(map)
                .setPopup(new mapboxgl.Popup().setHTML('<h1 class="popupCard">' + coords[key].name + '</h1>'))
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