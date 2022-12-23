// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.getElementById("Dropdown").addEventListener('click', function (event) {
    event.stopPropagation();
});


function myFunction() {
    var element = document.getElementById("navBar");
    var darkToggle = document.getElementById("darkToggle");
    const user = document.querySelector('.fa-user');
    const moon = document.querySelector('.fa-moon');
    const earth = document.querySelector('.fa-earth-americas');
    const brand = document.querySelector('.navbar-brand');
    const dropdown = document.querySelector('.dropdown-menu');
    var card = document.getElementById("card");
    var body = document.getElementById("body");
    var logoText = document.getElementById("logoText");
    var logoText1 = document.getElementById("logoText1");
    var slogan = document.getElementById("slogan");
    var logoTexts = document.getElementById("logoTexts");
    var logoTexts1 = document.getElementById("logoTexts1");
    var text = document.getElementById("text");
    var text1 = document.getElementById("text1");
    if (darkToggle.checked) {
        element.classList.add("navbar-dark");
        element.classList.add("bg-dark");
        dropdown.classList.add("bg-dark");
        element.classList.remove("navbar-light");
        element.classList.remove("bg-white");
        user.style.color = 'white';
        moon.style.color = 'white';
        earth.style.color = 'white';
        brand.style.width = '100px';
        logoText.style.fill = 'white';
        logoText1.style.fill = 'white';
        slogan.style.color = 'white';
        card.style.background = '#515151';
        body.classList.add("bg-dark");
        logoTexts.style.color = 'white';
        logoTexts1.style.color = 'white';
        text.style.color = 'white';
        text1.style.color = 'white';
    } else {
        element.classList.remove("navbar-dark");
        element.classList.remove("bg-dark");
        dropdown.classList.remove("bg-dark");
        element.classList.add("navbar-light");
        element.classList.add("bg-white");
        user.style.color = 'black';
        earth.style.color = 'black';
        moon.style.color = 'black';
        logoText.style.fill = 'black';
        logoText1.style.fill = 'black';
        slogan.style.color = 'black';
        card.style.background = '#D9D9D9';
        body.classList.remove("bg-dark");
        logoTexts.style.color = 'black';
        logoTexts1.style.color = 'black';
        text.style.color = 'black';
        text1.style.color = 'black';
    }
}