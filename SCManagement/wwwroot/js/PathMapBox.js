
mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';

const map = new mapboxgl.Map({
    container: 'map', // Specify the container ID
    style: 'mapbox://styles/mapbox/streets-v12', // Specify which map style to use
    center: [-8.8926, 38.5243], // Specify the starting position
    zoom: 14.5 // Specify the starting zoom
});

const layerList = document.getElementById('menu');
const inputs = layerList.getElementsByTagName('input');
const btnSave = document.getElementById('save-button');
const ev = document.getElementById("ev").value;
var response = "";
var newCoords = "";
var profile = "";


btnSave.onclick = function () {
    console.log(newCoords);
    
    try {
        //const query = await fetch(
        //    `https://api.mapbox.com/matching/v5/mapbox/${profile}/${coordinates}?geometries=geojson&radiuses=${radiuses}&steps=true&access_token=${mapboxgl.accessToken}`,
        //    { method: 'GET' }
        //);
        //const response = await query.json();
        
        if (response.code === 'Ok') {
            console.log("fffffffffffffff");

            $.ajax({
                type: 'POST',
                url: '/Events/ReceivePath',
                dataType: 'json',
                data: {
                    'coordinates': newCoords,
                    'profile': profile,
                    'evnt': ev
                },

            }).done(function (response) {
                console.log("Deu" + response);
                //btn.disabled = true;
                window.location.href = response.url;

            }).fail(function (jqXHR, textStatus, errorThrown) {
                console.log("Erro: " + textStatus + ", " + errorThrown);
                console.log("Resposta do servidor: " + jqXHR.responseText);
            });
        }
    } catch (error) {
        alert("fdsfsdfsdfsdfsdfsdfsdf");
    }
}

for (const input of inputs) {
    input.onclick = (layer) => {
        const layerId = layer.target.id;
        map.setStyle('mapbox://styles/mapbox/' + layerId);
    };
}

const draw = new MapboxDraw({
    // Instead of showing all the draw tools, show only the line string and delete tools
    displayControlsDefault: false,
    controls: {
        line_string: true,
        trash: true
    },
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
    removeRoute(); // Overwrite any existing layers

    const profile = 'walking'; // Set the profile

    // Get the coordinates
    const data = draw.getAll();
    console.log(data);
    const lastFeature = data.features.length - 1;
    console.log(lastFeature);
    const coords = data.features[lastFeature].geometry.coordinates;
    console.log(coords);
    // Format the coordinates
    newCoords = coords.join(';');
    console.log(newCoords);
    // Set the radius for each coordinate pair to 50 meters
    const radius = coords.map(() => 50);
    console.log(radius);
    getMatch(newCoords, radius, profile);
}

// Make a Map Matching request
async function getMatch(coordinates, radius, profile) {
    // Separate the radiuses with semicolons
    const radiuses = radius.join(';');
    console.log(radiuses);
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
    console.log(coords);
    // Draw the route on the map
    addRoute(coords);
    /*getInstructions(response.matchings[0]);*/
}

//function getInstructions(data) {
//    // Target the sidebar to add the instructions
//    const directions = document.getElementById('directions');
//    let tripDirections = '';
//    // Output the instructions for each step of each leg in the response object
//    for (const leg of data.legs) {
//        const steps = leg.steps;
//        for (const step of steps) {
//            tripDirections += `<li>${step.maneuver.instruction}</li>`;
//        }
//    }
//    directions.innerHTML = `<p><strong>Trip duration: ${Math.floor(
//        data.duration / 60
//    )} min.</strong></p><ol>${tripDirections}</ol>`;
//}

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

// If the user clicks the delete draw button, remove the layer if it exists
function removeRoute() {
    if (!map.getSource('route')) return;
    map.removeLayer('route');
    map.removeSource('route');
}