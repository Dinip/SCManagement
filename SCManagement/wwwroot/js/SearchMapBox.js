
mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';

const layerList = document.getElementById('menu');
const inputs = layerList.getElementsByTagName('input');
const addressElement = document.getElementById('address');
const newAd = document.getElementById('NewAd');
let coordX = document.getElementById('coordX').value;
let coordY = document.getElementById('coordY').value;

let marker;
let map;

if (coordX != "" && coordY != "") {
    coordX = parseFloat(document.getElementById('coordX').value.replace(',', '.'));
    coordY = parseFloat(document.getElementById('coordY').value.replace(',', '.'));

    console.log(coordX)
    console.log(coordY)
    map = new mapboxgl.Map({
        container: 'map', // Specify the container ID
        style: 'mapbox://styles/mapbox/outdoors-v12', // Specify which map style to use
        center: [coordX, coordY], // Specify the starting position
        zoom: 12 // Specify the starting zoom
    });
    addMarkers(coordX, coordY);
} else {
    map = new mapboxgl.Map({
        container: 'map',
        style: 'mapbox://styles/mapbox/streets-v12',
        center: [-8.896442, 38.533278],
        zoom: 13
    });
}


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
        if (coordX != "" && coordY != "") {
            marker.remove();
        }
    });
});

const btnSave = document.getElementById('save-button');

btnSave.onclick = function () {
    try {
        if (address != null) {
            console.log(address)
            let { text, geometry, context } = address;
            let addressCode = context.find(item => item.id.startsWith('postcode')).text;
            let city = context.find(item => item.id.startsWith('place')).text;
            let district = context.find(item => item.id.startsWith('region')).text;
            let country = context.find(item => item.id.startsWith('country')).text;
            let coord = geometry.coordinates;

            addressElement.value = JSON.stringify({
                CoordinateY: coord[1],
                CoordinateX: coord[0],
                ZipCode: addressCode,
                Street: text,
                City: city,
                District: district,
                Country: country,
            })
            newAd.innerHTML = strings.newAddress + ": " + text + "," + addressCode + "," + city + "," + district + "," + country;
        }

    } catch (error) {
        alert(strings.searchError)
    }

}


function addMarkers(coordX, coordY) {
    marker = new mapboxgl.Marker({ color: 'blue' })
        .setLngLat([coordX, coordY])
        .addTo(map);
}
