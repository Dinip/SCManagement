mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';
const map = new mapboxgl.Map({
    container: 'map', // Container ID
    style: 'mapbox://styles/mapbox/streets-v12', // Map style to use
    center: [-8.8926, 38.5243], // Starting position [lng, lat]
    zoom: 12 // Starting zoom level
});

const marker = new mapboxgl.Marker() // Initialize a new marker
    .setLngLat([-8.8926,38.5243]) // Marker [lng, lat] coordinates
    .addTo(map); // Add the marker to the map

const geocoder = new MapboxGeocoder({
    // Initialize the geocoder
    accessToken: mapboxgl.accessToken, // Set the access token
    mapboxgl: mapboxgl, // Set the mapbox-gl instance
    marker: false, // Do not use the default marker style
    placeholder: 'Search for places', // Placeholder text for the search bar
});

// Add the geocoder to the map
map.addControl(geocoder);

// After the map style has loaded on the page,
// add a source layer and default styling for a single point
map.on('load', () => {
    map.addSource('single-point', {
        'type': 'geojson',
        'data': {
            'type': 'FeatureCollection',
            'features': []
        }
    });

    map.addLayer({
        'id': 'point',
        'source': 'single-point',
        'type': 'circle',
        'paint': {
            'circle-radius': 10,
            'circle-color': '#448ee4'
        }
    });

    // Listen for the `result` event from the Geocoder // `result` event is triggered when a user makes a selection
    //  Add a marker at the result's coordinates
    geocoder.on('result', (event) => {
        map.getSource('single-point').setData(event.result.geometry);
        console.log("AAAAAAA"+event.result);
    });
});



//  Add Point

const nav = new mapboxgl.NavigationControl()
map.addControl(nav)

var directions = new MapboxDirections({
    accessToken: mapboxgl.accessToken
})

map.addControl(
    new MapboxDirections({
        accessToken: mapboxgl.accessToken
    }),
    'top-left'
);

const draw = new MapboxDraw({
    // Instead of showing all the draw tools, show only the line string and delete tools.
    displayControlsDefault: false,
    controls: {
        line_string: true,
        trash: true
    },
    // Set the draw mode to draw LineStrings by default.
    defaultMode: 'draw_line_string',
    styles: [
        //// Set the line style for the user-input coordinates.
        //{
        //    id: 'gl-draw-line',
        //    type: 'line',
        //    filter: ['all', ['==', '$type', 'LineString'], ['!=', 'mode', 'static']],
        //    layout: {
        //        'line-cap': 'round',
        //        'line-join': 'round'
        //    },
        //    paint: {
        //        'line-color': '#438EE4',
        //        'line-dasharray': [0.2, 2],
        //        'line-width': 4,
        //        'line-opacity': 0.7
        //    }
        //},
        // Style the vertex point halos.
        {
            id: 'gl-draw-polygon-and-line-vertex-halo-active',
            type: 'circle',
            filter: [
                'all',
                ['==', 'meta', 'vertex'],
                ['==', '$type', 'Point'],
                ['!=', 'mode', 'static']
            ],
            paint: {
                'circle-radius': 12,
                'circle-color': '#FFF'
            }
        },
        // Style the vertex points.
        {
            id: 'gl-draw-polygon-and-line-vertex-active',
            type: 'circle',
            filter: [
                'all',
                ['==', 'meta', 'vertex'],
                ['==', '$type', 'Point'],
                ['!=', 'mode', 'static']
            ],
            paint: {
                'circle-radius': 8,
                'circle-color': '#438EE4'
            }
        }
    ]
});

// Add the draw tool to the map.
map.addControl(draw);

// Use the coordinates you drew to make the Map Matching API request
function updateRoute() {
    // Set the profile
    /*const profile = 'driving';*/
    // Get the coordinates that were drawn on the map
    const data = draw.getAll();
    const lastFeature = data.features.length - 1;
    const coords = data.features[lastFeature].geometry.coordinates;
    // Format the coordinates
    const newCoords = coords.join(';');
    // Set the radius for each coordinate pair to 25 meters
    const radius = coords.map(() => 25);
    console.log(newCoords);
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
    const response = await query.json();
    // Handle errors
    if (response.code !== 'Ok') {
        alert(
            `${response.code} - ${response.message}.\n\nFor more information: https://docs.mapbox.com/api/navigation/map-matching/#map-matching-api-errors`
        );
        return;
    }
    // Get the coordinates from the response
    const coords = response.matchings[0].geometry;
    console.log(coords);
    // Code from the next step will go here
}

map.on('draw.create', updateRoute);
map.on('draw.update', updateRoute);
