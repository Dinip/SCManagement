﻿
mapboxgl.accessToken = 'pk.eyJ1IjoiZGF2aWRiZWxjaGlvciIsImEiOiJjbGMxMXZvdWYxMDFtM3RwOGNubTVjeGJyIn0.AIK0gyTLRqtnlYAeH5icxg';


const path = document.getElementById("path");
const hElevation = document.getElementById("hElevation");
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
        addRoute(coordsArray);
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
    dMore.textContent = totalAscent;
    dLess.textContent = totalDescent;
    createChartAltimetry(elevtsAux, distances);
}

getElevation(coordsArray);

// Draw the Map Matching route as a new layer on the map
function addRoute(coords) {

    // If a route is already loaded, remove it
    if (map.getSource('route')) {
        map.removeLayer('route');
        map.removeSource('route');
    } else {
        // Create a GeoJSON object with the coordinates
        const geojson = {
            type: 'FeatureCollection',
            features: [
                {
                    type: 'Feature',
                    geometry: {
                        type: 'LineString',
                        coordinates: coords,
                    },
                    properties: {},
                },
            ],
        };

        // Add the GeoJSON object as the data for the source
        map.addSource('route', {
            type: 'geojson',
            data: geojson,
        });

        // Add the layer for the line
        map.addLayer({
            id: 'route',
            type: 'line',
            source: 'route',
            layout: {
                'line-join': 'round',
                'line-cap': 'round',
            },
            paint: {
                'line-color': '#03AA46',
                'line-width': 8,
                'line-opacity': 0.8,
            },
        });

    }
    addMarkers(coords[0], coords[coords.length - 1]);

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


function createChartAltimetry(elevations, dists) {
    var ctx = document.getElementById('myChart').getContext('2d');
    var chart = new Chart(ctx, {
        type: 'line',
        data: {
            labels: dists,
            datasets: [{
                label: strings.altimetry,
                data: elevations,
                borderColor: 'rgb(255, 99, 132)',
                borderWidth: 1
            }]
        },
        options: {
            title: {
                display: true,
                text: strings.altimetryChart
            },
            scales: {
                yAxes: [{
                    type: 'linear',
                    position: 'left',
                    id: 'y-axis-1',
                    scaleLabel: {
                        display: true,
                        labelString: strings.altitude
                    }
                }],
                xAxes: [{
                    ticks: {
                        beginAtZero: true,
                        stepSize: 50
                    },
                    scaleLabel: {
                        display: true,
                        labelString: strings.distance
                    }
                }]
            },
            tooltips: {
                callbacks: {
                    label: function (tooltipItem, data) {
                        var datasetLabel = data.datasets[tooltipItem.datasetIndex].label || '';
                        var label = data.labels[tooltipItem.index];
                        var value = data.datasets[tooltipItem.datasetIndex].data[tooltipItem.index];
                        return `${datasetLabel}[ ${strings.distance}: ${label},  ${strings.altitude}: ${value}]`;
                    }
                }
            }
        }
    });
}