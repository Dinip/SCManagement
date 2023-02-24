

mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';


const map = new mapboxgl.Map({
    container: 'map', // Specify the container ID
    style: 'mapbox://styles/mapbox/streets-v12', // Specify which map style to use
    center: [-8.8926, 38.5243], // Specify the starting position
    zoom: 14.5 // Specify the starting zoom
});

const layerList = document.getElementById('menu');
const inputs = layerList.getElementsByTagName('input');
const path = document.getElementById("path");


for (const input of inputs) {
    input.onclick = (layer) => {
        const layerId = layer.target.id;
        map.setStyle('mapbox://styles/mapbox/' + layerId);
    };
}


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
    const coordsString = coordinates;
    const coordsArrayString = coordsString.split(';');
    const coodrsArray = coordsArrayString.map(coord => {
        const [longitude, latitude] = coord.split(',');
        return [parseFloat(longitude), parseFloat(latitude)];
    });

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
    console.log(response);
    // Handle errors
    if (response.code !== 'Ok') {
        alert(
            `${response.code} - ${response.message}.\n\nFor more information: https://docs.mapbox.com/api/navigation/map-matching/#map-matching-api-errors`
        );
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

