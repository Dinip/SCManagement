﻿
mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';


const path = document.getElementById("path");
let map;

if (path.value !== 'null') {
    let coordsString = path.value;
    let coordsArrayString = coordsString.split(';');
    let coordsArray = coordsArrayString.map(coord => {
        const [longitude, latitude] = coord.split(',');
        return [parseFloat(longitude), parseFloat(latitude)];
    });

    map = new mapboxgl.Map({
        container: 'map', // Specify the container ID
        style: 'mapbox://styles/mapbox/outdoors-v12', // Specify which map style to use
        center: [coordsArray[0][0], coordsArray[0][1]], // Specify the starting position
        zoom: 12 // Specify the starting zoom
    });

    map.on('load', function () {
        map.resize();
        getNewMatch(path.value);
    });


    // Make a Map Matching request
    async function getNewMatch(coordinates) {
        const profile = 'walking';

        // Set the radius for each coordinate pair to 50 meters
        const radius = coordsArray.map(() => 50);

        // Separate the radiuses with semicolons
        const radiuses = radius.join(';');

        // Create the query
        const query = await fetch(
            `https://api.mapbox.com/matching/v5/mapbox/${profile}/${coordinates}?geometries=geojson&radiuses=${radiuses}&steps=true&access_token=${mapboxgl.accessToken}`,
            { method: 'GET' }
        );
        response = await query.json();
        // Handle errors
        if (response.code !== 'Ok') {
            errorMessage(response.code);
            return;
        }
        const coords = response.matchings[0].geometry;

        // Draw the route on the map
        addRoute(coords);
    }

} else {
    map = new mapboxgl.Map({
        container: 'map', // Specify the container ID
        style: 'mapbox://styles/mapbox/outdoors-v12', // Specify which map style to use
        center: [-8.8926, 38.5243], // Specify the starting position
        zoom: 14.5 // Specify the starting zoom
    });

    map.on('load', () => {
        map.resize();
    });

}


const btnSave = document.getElementById('save-button');
const ev = document.getElementById("ev");
let addressByPath = document.getElementById("AddressByPath");
const btnDraw = document.getElementsByClassName("mapbox-gl-draw_ctrl-draw-btn");

let erMessage = "";
let newCoords = null;
let initialMarker = null;
let endMarker = null;

const geocoder = new MapboxGeocoder({
    accessToken: mapboxgl.accessToken,
    mapboxgl: mapboxgl,
    marker: false,
    placeholder: 'Digite um endereço ou localização'
});

map.addControl(geocoder, 'top-left');

btnSave.onclick = function () {
    if (newCoords != null) {
        ev.value = newCoords;
        btnSave.classList.add("d-none");
    }
}

let draw = new MapboxDraw({
    // Instead of showing all the draw tools, show only the line string and delete tools
    displayControlsDefault: false,
    controls: {
        line_string: true,
        trash: true
    }, userProperties: true,
    // Set the draw mode to draw LineStrings by default
    defaultMode: 'draw_line_string',
    styles: [
        // Set the line style for the user-input coordinates
        {
            'id': 'gl-draw-line',
            'type': 'line',
            'filter': [
                'all',
                ['==', '$type', 'LineString'],
                ['!=', 'mode', 'static']
            ],
            'layout': {
                'line-cap': 'round',
                'line-join': 'round'
            },
            'paint': {
                'line-color': '#438EE4',
                'line-dasharray': [0.2, 2],
                'line-width': 2,
                'line-opacity': 0.7
            }
        },
        // Style the vertex point halos
        {
            'id': 'gl-draw-polygon-and-line-vertex-halo-active',
            'type': 'circle',
            'filter': [
                'all',
                ['==', 'meta', 'vertex'],
                ['==', '$type', 'Point'],
                ['!=', 'mode', 'static']
            ],
            'paint': {
                'circle-radius': 12,
                'circle-color': '#FFF'
            }
        },
        // Style the vertex points
        {
            'id': 'gl-draw-polygon-and-line-vertex-active',
            'type': 'circle',
            'filter': [
                'all',
                ['==', 'meta', 'vertex'],
                ['==', '$type', 'Point'],
                ['!=', 'mode', 'static']
            ],
            'paint': {
                'circle-radius': 8,
                'circle-color': '#438EE4'
            }
        }
    ]
});


// Add the draw tool to the map
map.addControl(draw);

// Add create, update, or delete actions
map.on('draw.create', updateRoute);
map.on('draw.update', updateRoute);
map.on('draw.delete', removeRoute);


// Use the coordinates you just drew to make the Map Matching API request
function updateRoute() {
    const data = draw.getAll();

    if (initialMarker != null && endMarker != null) {
        initialMarker.remove();
        endMarker.remove();
    }

    btnSave.classList.remove("d-none");

    const profile = 'walking'; // Set the profile

    // Get the coordinates

    const lastFeature = data.features.length - 1;
    const coords = data.features[lastFeature].geometry.coordinates;
    if (coords.length < 2) return;
    data.features[0].geometry.coordinates = [coords];

    // Format the coordinates
    newCoords = coords.join(';');

    // Set the radius for each coordinate pair to 50 meters
    const radius = coords.map(() => 50);
    getMatch(newCoords, radius, profile);
}

// Make a Map Matching request
async function getMatch(coordinates, radius, profile) {
    // Separate the radiuses with semicolons
    const radiuses = radius.join(';');
    // Create the query
    const query = await fetch(
        `https://api.mapbox.com/matching/v5/mapbox/${profile}/${coordinates}?geometries=geojson&radiuses=${radiuses}&steps=true&access_token=${mapboxgl.accessToken}`,
        { method: 'GET' }
    );
    response = await query.json();

    // Handle errors
    if (response.code !== 'Ok') {
        errorMessage(response.code);
        removeRoute();
        draw.deleteAll();
        return;
    }
    const coords = response.matchings[0].geometry;
    // Draw the route on the map
    addRoute(coords);
}


// Draw the Map Matching route as a new layer on the map
function addRoute(coords) {
    // If a route is already loaded, remove it
    if (map.getSource('trace')) {
        let aux

        if (draw.getAll().features[1] === undefined) {
            aux = draw.getAll().features[0];
        } else {
            aux = draw.getAll().features[1];
            draw.delete(draw.getAll().features[0].id);
        }

        map.getSource("trace").setData({
            "type": "FeatureCollection",
            "features": aux
        });

        map.removeLayer("tracee");
        map.removeSource("tracee");
        map.addLayer({
            'id': 'tracee',
            'type': 'line',
            'source': {
                'type': 'geojson',
                'data': {
                    'type': 'Feature',
                    'properties': {},
                    'geometry': coords
                }
            },
            'layout': {
                'line-join': 'round',
                'line-cap': 'round'
            },
            'paint': {
                'line-color': '#03AA46',
                'line-width': 8,
                'line-opacity': 0.8
            },

        });

    } else {
        map.addSource('trace', {
            type: 'geojson',
            data: {
                "type": "FeatureCollection",
                "features": [{
                    "type": "Feature",
                    "properties": {},
                    "geometry": {
                        "type": "Point",
                        "coordinates": coords
                    }
                }]
            }
        });

        map.addLayer({
            'id': 'tracee',
            'type': 'line',
            'source': {
                'type': 'geojson',
                'data': {
                    'type': 'Feature',
                    'properties': {},
                    'geometry': coords
                }
            },
            'layout': {
                'line-join': 'round',
                'line-cap': 'round'
            },
            'paint': {
                'line-color': '#03AA46',
                'line-width': 8,
                'line-opacity': 0.8
            }
        });
    }
    addMarkers(coords.coordinates[0], coords.coordinates[coords.coordinates.length - 1]);
    getCityAndCountryFromCoordinates(coords.coordinates[0][1], coords.coordinates[0][0], mapboxgl.accessToken)
}

// If the user clicks the delete draw button, remove the layer if it exists
function removeRoute() {
    if (!map.getSource('trace')) {
        newCoords = null;
        return;
    }
    map.removeLayer('tracee');
    map.removeSource('tracee');
    map.removeSource('trace');

    if (initialMarker != null && endMarker != null) {
        initialMarker.remove();
        endMarker.remove();
    }

    btnSave.classList.remove("d-none");
    newCoords = null;
    ev.value = newCoords;
    addressByPath.value = null;

}

function addMarkers(initialCoord, endCoord) {
    initialMarker = new mapboxgl.Marker({ color: 'green' })
        .setLngLat([initialCoord[0], initialCoord[1]])
        .setPopup(new mapboxgl.Popup().setHTML(strings.start))
        .addTo(map);
    
    endMarker = new mapboxgl.Marker({ color: 'red' })
        .setLngLat([endCoord[0], endCoord[1]])
        .setPopup(new mapboxgl.Popup().setHTML(strings.end))
        .addTo(map);
} 

function errorMessage(codeMessage) {
    if (codeMessage == "NoMatch") {
        erMessage = strings.noMatch;
        alert(erMessage);
    } else if (codeMessage == "NoSegment") {
        erMessage = strings.noSegment;
        alert(erMessage);
    } else if (codeMessage == "TooManyCoordinates") {
        erMessage = strings.tooManyCoordinates;
        alert(erMessage);
    } else if (codeMessage == "ProfileNotFound") {
        erMessage = strings.profileNotFound;
        alert(erMessage);
    } else if (codeMessage == "InvalidInput") {
        erMessage = strings.invalidInput;
        alert(erMessage);
    } else if (codeMessage == "Other") {
        erMessage = strings.other;
        alert(erMessage);
    }
}


async function getCityAndCountryFromCoordinates(latitude, longitude, accessToken) {
    const url = `https://api.mapbox.com/geocoding/v5/mapbox.places/${longitude},${latitude}.json?ypes=poi,address,region,district,place,country&access_token=${accessToken}`;

    try {
        const response = await fetch(url);
        const data = await response.json();
        const address = data.features[0].place_name;
        addressByPath.value = address;
    } catch (error) {
        errorMessage("Other")
        removeRoute();
        draw.deleteAll();
        return;
    }
}

