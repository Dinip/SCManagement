
mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';


const path = document.getElementById("path");
const hElevation = document.getElementById("hElevation");
const lElevation = document.getElementById("lElevation");
const tDistance = document.getElementById("tDistance");
const dMore = document.getElementById("dMore");
const dLess = document.getElementById("dLess");


let coordsString = path.value;
let coordsArrayString = coordsString.split(';');
let coordsArray = coordsArrayString.map(coord => {
    const [longitude, latitude] = coord.split(',');
    return [parseFloat(longitude), parseFloat(latitude)];
});


let highestElevation = -Infinity;
let lowestElevation = Infinity;
let totalAscent = 0;
let totalDescent = 0;
let prevElevation;



const map = new mapboxgl.Map({
    container: 'map', // Specify the container ID
    style: 'mapbox://styles/mapbox/outdoors-v12', // Specify which map style to use
    center: [coordsArray[0][0], coordsArray[0][1]], // Specify the starting position
    zoom: 12 // Specify the starting zoom
});


map.on('load', function () {
    map.resize();
    if (path.value != null) {
        getMatch(path.value);
    } 
});


// convert the coordinates into an object of type LineString from Turf.js
//Get total Distance in Path
let lineString = turf.lineString(coordsArray);
let length = turf.lineDistance(lineString, 'kilometers');
lineString.properties.distance = length
tDistance.textContent = lineString.properties.distance.toFixed(2);

// Split the path into smaller segments
const lineSegments = [];
for (let i = 0; i < coordsArray.length - 1; i++) {
    const segment = turf.lineString([coordsArray[i], coordsArray[i + 1]]);
    lineSegments.push(segment);
}

// Calcule a distância de cada segmento e a distância acumulada ao longo do caminho
const distances = [0];
let cumulativeDistance = 0;
lineSegments.forEach(segment => {
    const length = turf.lineDistance(segment, 'kilometers');
    cumulativeDistance += length;
    distances.push(cumulativeDistance.toFixed(3));
});

const elevtsAux = [];

//Get the max and min elevation in a path
async function getElevation(coordsArray) {
    
    
    for (let i = 0; i < coordsArray.length; i++) {
        // Make the API request for each coordinate.
        const query = await fetch(
            `https://api.mapbox.com/v4/mapbox.mapbox-terrain-v2/tilequery/${coordsArray[i][0]},${coordsArray[i][1]}.json?layers=contour&limit=50&access_token=${mapboxgl.accessToken}`,
            { method: 'GET' }
        );
        if (query.status !== 200) continue;
        const data = await query.json();

        // Get all the returned features.
        const allFeatures = data.features;
        // For each returned feature, add elevation data to the elevations array.
        const elevations = allFeatures.map((feature) => feature.properties.ele);
        
        // Find the largest and smallest elevation in the elevations array.
        const highestCoordElevation = Math.max(...elevations);
        const lowestCoordElevation = Math.min(...elevations);

        // Update the highest and lowest elevation variables if necessary.
        if (highestCoordElevation > highestElevation) {
            highestElevation = highestCoordElevation;
        }
        if (lowestCoordElevation < lowestElevation) {
            lowestElevation = lowestCoordElevation;
        }

        if (prevElevation !== undefined) {
            const elevationDiff = highestCoordElevation - prevElevation;
            if (elevationDiff > 0) {
                totalAscent += elevationDiff;
            } else {
                totalDescent -= elevationDiff;
            }
        }
        prevElevation = highestCoordElevation;
        elevtsAux.push(highestCoordElevation);
    }

    hElevation.textContent = highestElevation;
    lElevation.textContent = lowestElevation;
    dMore.textContent = totalAscent;
    dLess.textContent = totalDescent;
    createChartAltimetry(elevtsAux, distances);
}

getElevation(coordsArray);



// Make a Map Matching request
async function getMatch(coordinates) {
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
    addMarkers(coords.coordinates[0], coords.coordinates[coords.coordinates.length - 1]);

}

function addMarkers(initialCoord, endCoord) {
    initialMarker = new mapboxgl.Marker({ color: 'green' })
        .setLngLat([initialCoord[0], initialCoord[1]])
        .setPopup(new mapboxgl.Popup().setHTML('Inicio'))
        .addTo(map);

    endMarker = new mapboxgl.Marker({ color: 'red' })
        .setLngLat([endCoord[0], endCoord[1]])
        .setPopup(new mapboxgl.Popup().setHTML('Fim'))
        .addTo(map);
}


function createChartAltimetry(elevations,dists) {
    var ctx = document.getElementById('myChart').getContext('2d');
    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: dists,
            datasets: [{
                label:"Grafico De Altimetria",
                data: elevations,
                
                borderColor: 'rgb(255, 99, 132)',
                
                borderWidth: 1
            }]
        }    
    });
}

