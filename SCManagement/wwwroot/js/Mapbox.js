
mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';

window.onload = (event) => {
    navigator.geolocation.getCurrentPosition(function (position) {
        loadMap(position);
    });

    $.ajax({
        type: 'GET',
        url: '/Clubs/EXEMPLO'
    }).done(function (response) {
        console.log("Sucess");
        console.log(response);
        loadMap(response);
    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log("Erro: " + textStatus + ", " + errorThrown);
        console.log("Resposta do servidor: " + jqXHR.responseText);
    });
};


function loadMap(coordinates) {
    let map = new mapboxgl.Map({
        container: 'map', // container ID
        style: 'mapbox://styles/mapbox/streets-v12',
        zoom: 10,
        center: [-8.8926, 38.5243]
    });

    // Usa a API de Geolocalização para obter as coordenadas do utilizador
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition((position) => {
            // Define o centro do mapa para a posição do utilizador
            map.setCenter([position.coords.longitude, position.coords.latitude]);
        }, (error) => {
            console.log(error);
        });
    }

    const geocoder = new MapboxGeocoder({
        accessToken: mapboxgl.accessToken,
        mapboxgl: mapboxgl,
        marker: false
    });

    map.addControl(geocoder, 'top-left');

    // adiciona um marcador na localização atual do utilizador
    //navigator.geolocation.getCurrentPosition(function (position) {
    //    var marker = new mapboxgl.Marker()
    //        .setLngLat([position.coords.longitude, position.coords.latitude])
    //        .addTo(map);
    //});

    map.on('load', () => {
        map.resize();
        var coords = coordinates;
        Object.keys(coords).forEach(function (key) {
            console.log(coords[key]);
            console.log(coords[key].coordinateX);
            new mapboxgl.Marker()
                .setLngLat([coords[key].coordinateX, coords[key].coordinateY])
                .addTo(map);
        });

        //coordinates.forEach(coord => {
        //    console.log("AQUI1");
        //    console.log(coord.CoordinateX);
        //    console.log("AQUI2");
        //    console.log(coord.CoordinateY);
        //    new mapboxgl.Marker()
        //        .setLngLat([coord.CoordinateX, coord.CoordinateY])
        //        .addTo(map);
        //});
    });
}