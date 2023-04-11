
mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';

const layerList = document.getElementById('menu');
const inputs = layerList.getElementsByTagName('input');
const addressElement = document.getElementById('address');
const newAd = document.getElementById('NewAd');
let coordX = document.getElementById('coordX').value;
let coordY = document.getElementById('coordY').value;
let tradPlaceholder = document.getElementById('tradPlaceholder');

let markers;
let map;

if (coordX != "" && coordY != "") {
    coordX = parseFloat(document.getElementById('coordX').value.replace(',', '.'));
    coordY = parseFloat(document.getElementById('coordY').value.replace(',', '.'));

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
    marker: false,
    placeholder: tradPlaceholder.value,
});

map.addControl(geocoder, 'top-left');


let errorMessage;
let coord;

map.on('load', () => {
    MarkerWithAddress();
});

let address = null;

function addMarkers(coordX, coordY) {
    markers = new mapboxgl.Marker({ color: '#00639A' })
        .setLngLat([coordX, coordY])
        .addTo(map);
}

let marker = null;

function MarkerWithAddress() {
    map.on('click', function (e) {
        // Capture the coordinates of the clicked point
        if (marker !== null) {
            marker.remove();
        }
        coord = e.lngLat.toArray();

        // Add the marker to the map
        marker = new mapboxgl.Marker({ color: "#00639A" })
            .setLngLat([coord[0], coord[1]])
            .addTo(map);

        // put address
        addAddress(e);
    });
}

function addAddress(epoint) {
    // Send an HTTP request to the Geocoding API
    let geocodeUrl = 'https://api.mapbox.com/geocoding/v5/mapbox.places/' + coord[0] + ',' + coord[1] + '.json?ypes=poi,address,region,district,place,country&access_token=' + mapboxgl.accessToken;

    fetch(geocodeUrl)
        .then(response => response.json())
        .then(data => {
            let features = null;

            try {
                if (map.getStyle().name === 'Mapbox Satellite Streets') {

                    address = data.features[0].place_name;
                    if (coordX != "" && coordY != "") {
                        markers.remove();
                    }
                    try {
                        if (address != null) {
                            console.log(address)
                            addressElement.value = JSON.stringify({
                                CoordinateX: coord[0],
                                CoordinateY: coord[1],
                                AddressString: address,

                            })
                            $("#modal").hide();
                            newAd.innerHTML = strings.newAddress + ": " + address;
                        }

                    } catch (error) {
                        $(".toast").show();
                        document.getElementById('alertText').innerHTML = strings.resultError;
                    }

                } else {
                    features = map.queryRenderedFeatures(epoint.point, { layers: ['water'] });
                    if (features.length > 0) {
                        errorRemoveMarker(strings.searchError);
                    } else {
                        // Extract the address information from the JSON response
                        if (data.features && data.features.length > 0) {
                            address = data.features[0].place_name;
                            if (coordX != "" && coordY != "") {
                                markers.remove();
                            }
                            try {
                                if (address != null) {
                                    console.log(address)
                                    addressElement.value = JSON.stringify({
                                        CoordinateX: coord[0],
                                        CoordinateY: coord[1],
                                        AddressString: address,

                                    })
                                    $("#modal").hide();
                                    newAd.innerHTML = strings.newAddress + ": " + address;
                                }

                            } catch (error) {
                                $(".toast").show();
                                document.getElementById('alertText').innerHTML = strings.resultError;
                            }
                        }
                    }
                }
            } catch (error) {
                errorRemoveMarker(strings.resultError);
                address = null;
            }

        });
}


function errorRemoveMarker(errorMessage) {
    $(".toast").show();
    document.getElementById('alertText').innerHTML = errorMessage;
    marker.remove();
    address = null;
    newAd.innerHTML = "";
}

