
mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';
const map = new mapboxgl.Map({
    container: 'map',
    style: 'mapbox://styles/mapbox/streets-v12',
    center: [-8.896442, 38.533278],
    zoom: 13
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
    types: 'address',
    layers: ['address']
});

map.addControl(geocoder, 'top-left');

let address;
let errorMessage;

map.on('load', () => {
    // Listen for the `geocoder.input` event that is triggered when a user
    // makes a selection
    geocoder.on('result', (event) => {
        address = event.result;
        
    });
});

window.onload = function () {

    let btn = document.getElementById("btnSave");

    btn.onclick = function () {

        try {
            if (address != null) {

                let { text, geometry, context } = address;
                let addressCode = context[0].text;
                let city = context[1].text;
                let district = context[2].text;
                let country = context[3].text;
                let coord = geometry.coordinates;

                $.ajax({
                    type: 'POST',
                    url: '/MyClub/ReceiveAddress',
                    dataType: 'json',
                    data: {
                        Address:
                        {
                            CoordinateY: coord[1],
                            CoordinateX: coord[0],
                            ZipCode: addressCode,
                            Street: text,
                            City: city,
                            District: district,
                            Country: country,
                        }
                    },
                   
                }).done(function (response) {
                    btn.disabled = true;
                    window.location.href = response.url;
                        
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    console.log("Erro: " + textStatus + ", " + errorThrown);
                    console.log("Resposta do servidor: " + jqXHR.responseText);
                });
            }
        } catch (error) {
            errorMessage = "Terá de inserir uma localização com rua incluida";
            alert(errorMessage);
        }
    }
};
