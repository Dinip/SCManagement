﻿
mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';


const path = document.getElementById("path");

let coordsString = path.value;
let coordsArrayString = coordsString.split(';');
let coodrsArray = coordsArrayString.map(coord => {
    const [longitude, latitude] = coord.split(',');
    return [parseFloat(longitude), parseFloat(latitude)];
});


const map = new mapboxgl.Map({
    container: 'map', // Specify the container ID
    style: 'mapbox://styles/mapbox/streets-v12', // Specify which map style to use
    center: [coodrsArray[0][0], coodrsArray[0][1]], // Specify the starting position
    zoom: 12 // Specify the starting zoom
});


map.on('load', function () {
    map.resize();
    if (path.value != null) {
        getMatch(path.value);
    } else {
        console.log("erro");
    }
});




// Make a Map Matching request
async function getMatch(coordinates) {
    const profile = 'walking';
    
    // Set the radius for each coordinate pair to 50 meters
    const radius = coodrsArray.map(() => 50);

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
        if (response.code == "NoMatch") {
            errorMessage = "The input did not produce any matches, or the waypoints requested were not found in the resulting match. features will be an empty array.";
            alert(errorMessage);
        } else if (response.code == "NoSegment") {
            errorMessage = "No road segment could be matched for one or more coordinates within the supplied radiuses. Check for coordinates that are too far away from a road."
            alert(errorMessage);
        } else if (response.code == "TooManyCoordinates") {
            errorMessage = "There are more than 100 points in the request."
            alert(errorMessage);
        } else if (response.code == "ProfileNotFound") {
            errorMessage = "Needs to be a valid profile (mapbox/driving, mapbox/driving-traffic, mapbox/walking, or mapbox/cycling).";
            alert(errorMessage);
        } else if (response.code == "InvalidInput") {
            errorMessage = "message will hold an explanation of the invalid input.";
            alert(errorMessage);
        }    
        return;
    }
    const coords = response.matchings[0].geometry;

    // Draw the route on the map
    addRoute(coords);
}


// Draw the Map Matching route as a new layer on the map
function addRoute(coords) {
    
    // If a route is already loaded, remove it
    if (map.getSource('route')) {
        map.removeLayer('route');
        map.removeSource('route');
    } else {
        map.addLayer({
            'id': 'route',
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
}

