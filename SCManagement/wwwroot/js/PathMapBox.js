
mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';

const map = new mapboxgl.Map({
    container: 'map', // Specify the container ID
    style: 'mapbox://styles/mapbox/streets-v12', // Specify which map style to use
    center: [-8.8926, 38.5243], // Specify the starting position
    zoom: 14.5 // Specify the starting zoom
});

map.on('load', () => {
    map.resize();
});

const layerList = document.getElementById('menu');
const inputs = layerList.getElementsByTagName('input');
const btnSave = document.getElementById('save-button');
const ev = document.getElementById("ev");
const btnDraw = document.getElementsByClassName("mapbox-gl-draw_ctrl-draw-btn");



var response = "";
var newCoords = "";
var profile = "";

btnSave.onclick = function () {
    console.log(newCoords);

    if (newCoords != "") {
        ev.value = newCoords;
        btnSave.classList.add("d-none");
    }
}

for (const input of inputs) {
    input.onclick = (layer) => {
        const layerId = layer.target.id;
        map.setStyle('mapbox://styles/mapbox/' + layerId);
    };
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

    btnSave.classList.remove("d-none");

    //removeRoute(); // Overwrite any existing layers

    const profile = 'walking'; // Set the profile

    // Get the coordinates

    const lastFeature = data.features.length - 1;
    const coords = data.features[lastFeature].geometry.coordinates;
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
        alert(
            `${response.code} - ${response.message}.\n\nFor more information: https://docs.mapbox.com/api/navigation/map-matching/#map-matching-api-errors`
        );
        return;
    }
    const coords = response.matchings[0].geometry;
    // Draw the route on the map
    addRoute(coords);
}

const x = 0;

// Draw the Map Matching route as a new layer on the map
function addRoute(coords) {
    // If a route is already loaded, remove it
    console.log(coords);
    if (map.getSource('trace')) {
        let aux
        if (draw.getAll().features[1] === undefined) {
            aux = draw.getAll().features[0];
        } else {
            aux = draw.getAll().features[1];
        }
       
       
      
        let updatedGeoJSONData =
        {
            "type": "FeatureCollection",
            "features": [aux]
        };
        draw.getAll().features = [aux];

        map.getSource("trace").setData(updatedGeoJSONData);

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
            }
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
}

// If the user clicks the delete draw button, remove the layer if it exists
function removeRoute() {
    if (!map.getSource('trace')) return;
    map.removeLayer('tracee');
    map.removeSource('trace');

}