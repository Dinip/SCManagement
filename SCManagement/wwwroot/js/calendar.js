﻿// Setup the calendar with the current date
$(document).ready(function () {
    var date = new Date();
    var today = date.getDate();
    //Request to DB for all events
    $.ajax({
        type: 'GET',
        url: '/Events/GetAllEvents',
        dataType: 'json'
    },
    ).done(function (response) {
        response.forEach((event) => {
            new_event_json(event)
        })

        // Set click handlers for DOM elements
        $(".right-button").click({ date: date }, next_year);
        $(".left-button").click({ date: date }, prev_year);
        $(".month").click({ date: date }, month_click);
        $("#add-button").click({ date: date }, new_event);
        // Set current month as active
        $(".months-row").children().eq(date.getMonth()).addClass("active-month");
        init_calendar(date);
        var events = check_events(today, date.getMonth() + 1, date.getFullYear());
        show_events(events, months[date.getMonth()], today);

    }).fail(function (jqXHR, textStatus, errorThrown) {
        console.log("Erro: " + textStatus + ", " + errorThrown);
        console.log("Resposta do servidor: " + jqXHR.responseText);
    });


});

// Initialize the calendar by appending the HTML dates
function init_calendar(date) {
    $(".tbody").empty();
    $(".events-container").empty();
    var calendar_days = $(".tbody");
    var month = date.getMonth();
    var year = date.getFullYear();
    var day_count = days_in_month(month, year);
    var row = $("<tr class='table-row'></tr>");
    var today = date.getDate();
    // Set date to 1 to find the first day of the month
    date.setDate(1);
    var first_day = date.getDay();
    // 35+firstDay is the number of date elements to be added to the dates table
    // 35 is from (7 days in a week) * (up to 5 rows of dates in a month)
    for (var i = 0; i < 35 + first_day; i++) {
        // Since some of the elements will be blank, 
        // need to calculate actual date from index
        var day = i - first_day + 1;
        // If it is a sunday, make a new row
        if (i % 7 === 0) {
            calendar_days.append(row);
            row = $("<tr class='table-row'></tr>");
        }
        // if current index isn't a day in this month, make it blank
        if (i < first_day || day > day_count) {
            var curr_date = $("<td class='table-date nil text-to-dark'>" + "</td>");
            row.append(curr_date);
        }
        else {
            var curr_date = $("<td class='table-date text-to-dark'>" + day + "</td>");
            var events = check_events(day, month + 1, year);
            if (today === day && $(".active-date").length === 0) {
                curr_date.addClass("active-date");
                show_events(events, months[month], day);
            }
            // If this date has any events, style it with .event-date
            if (events.length !== 0) {
                curr_date.addClass("event-date");
            }
            // Set onClick handler for clicking a date
            curr_date.click({ events: events, month: months[month], day: day }, date_click);
            row.append(curr_date);
        }
    }
    // Append the last row and set the current year
    calendar_days.append(row);
    $(".year").text(year);
    if (localStorage.getItem("theme") == "dark") {
        let calendarBG = document.getElementById('calendar-container');
        let eventCardBG = document.getElementById('event-card');
        let texts = document.getElementsByClassName('text-to-dark');
        if (calendarBG) {
            calendarBG.classList.add("bg-dark");
        }

        if (eventCardBG) {
            eventCardBG.classList.add("bg-dark");
        }
        for (let i = 0; i < texts.length; i++) {
            texts[i].style.color = 'white';
        }
    }
}

// Get the number of days in a given month/year
function days_in_month(month, year) {
    var monthStart = new Date(year, month, 1);
    var monthEnd = new Date(year, month + 1, 1);
    return (monthEnd - monthStart) / (1000 * 60 * 60 * 24);
}

// Event handler for when a date is clicked
function date_click(event) {
    $(".events-container").show(250);
    $("#dialog").hide(250);
    $(".active-date").removeClass("active-date");
    $(this).addClass("active-date");
    show_events(event.data.events, event.data.month, event.data.day);
};

// Event handler for when a month is clicked
function month_click(event) {
    $(".events-container").show(250);
    $("#dialog").hide(250);
    var date = event.data.date;
    $(".active-month").removeClass("active-month");
    $(this).addClass("active-month");
    var new_month = $(".month").index(this);
    date.setMonth(new_month);
    init_calendar(date);
}

// Event handler for when the year right-button is clicked
function next_year(event) {
    $("#dialog").hide(250);
    var date = event.data.date;
    var new_year = date.getFullYear() + 1;
    $("year").html(new_year);
    date.setFullYear(new_year);
    init_calendar(date);
}

// Event handler for when the year left-button is clicked
function prev_year(event) {
    $("#dialog").hide(250);
    var date = event.data.date;
    var new_year = date.getFullYear() - 1;
    $("year").html(new_year);
    date.setFullYear(new_year);
    init_calendar(date);
}

// Event handler for clicking the new event button
function new_event() {
    window.location.href = "/Events/Create"
}

// Adds a json event to event_data
function new_event_json(paramEvent) {
    let { id, startDate, translate, clubName } = paramEvent;
    let eventDate = new Date(startDate)
    var event = {
        "id": id,
        "occasion": translate,
        "clubName": clubName,
        "year": eventDate.getFullYear(),
        "month": eventDate.getMonth() + 1,
        "day": eventDate.getDate(),
    };
    event_data["events"].push(event);
}

// Display all events of the selected date in card views
function show_events(events, month, day) {
    // Clear the dates container
    $(".events-container").empty();
    $(".events-container").show(250);
    // If there are no events for this date, notify the user
    if (events.length === 0) {
        var event_card = $("<div class='event-card' id='event-card'></div>");
        var event_name = $("<div class='event-name text-to-dark'>" + strings.noEvents + " " + month + " " + day + ".</div>");
        $(event_card).css({ "border-left": "10px solid #00639A" });
        $(event_card).append(event_name);
        $(".events-container").append(event_card);
    }
    else {
        // Go through and add each event as a card to the events container
        for (var i = 0; i < events.length; i++) {
            var event_card = $("<div style='cursor:pointer' onclick='DetailsEvent("+events[i]["id"]+")' class='event-card'></div>");
            var event_name = $("<div class='event-name text-to-dark'>" + events[i]["occasion"] + ":</div>");
            var event_count = $("<div class='event-count'> Sponsor by " + events[i]["clubName"] + "</div>");
            if (localStorage.getItem("theme") == "dark") {
                var event_card = $("<div style='cursor:pointer' onclick='DetailsEvent(" + events[i]["id"] + ")' class='event-card bg-dark'></div>");
                var event_name = $("<div class='event-name text-to-dark' style='color: white'>" + events[i]["occasion"] + ":</div>");
                var event_count = $("<div class='event-count' style='color: white'> Sponsor by " + events[i]["clubName"] + "</div>");
            } else {
                var event_card = $("<div style='cursor:pointer' onclick='DetailsEvent(" + events[i]["id"] + ")' class='event-card'></div>");
                var event_name = $("<div class='event-name text-to-dark'>" + events[i]["occasion"] + ":</div>");
                var event_count = $("<div class='event-count'> Sponsor by " + events[i]["clubName"] + "</div>");
            }
            $(event_card).append(event_name).append(event_count);
            $(".events-container").append(event_card);
        }
    }
    if (localStorage.getItem("theme") == "dark") {
        let eventCardBG = document.getElementById('event-card');
        let texts = document.getElementsByClassName('text-to-dark');
        if (eventCardBG) {
            eventCardBG.classList.add("bg-dark");
        }
        for (let i = 0; i < texts.length; i++) {
            texts[i].style.color = 'white';
        }
    }
}

function DetailsEvent(id) {
    window.location.href = "/Events/Details/" + id;
}

// Checks if a specific date has any events
function check_events(day, month, year) {
    var events = [];
    for (var i = 0; i < event_data["events"].length; i++) {
        var event = event_data["events"][i];
        if (event["day"] === day &&
            event["month"] === month &&
            event["year"] === year) {
            events.push(event);
        }
    }
    return events;
}

// Given data for events in JSON format
var event_data = {
    "events": [
    ]
};

const months = [
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December"
];