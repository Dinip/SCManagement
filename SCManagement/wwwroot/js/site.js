
document.getElementById("Dropdown").addEventListener('click', function (event) {
    event.stopPropagation();
});

document.getElementById("Dropdown1").addEventListener('click', function (event) {
    event.stopPropagation();
});

const dropdown = document.querySelector('.dropdown');
const dropdownM = document.querySelector('.dropdown-menu');

if (window.innerWidth <= 768) {
    var elem = document.getElementById('dropdownMenuButton1');

    elem.parentNode.removeChild(elem);

    dropdown.classList.remove("dropdown");
    dropdownM.classList.remove("dropdown-menu");
} else {
    dropdown.classList.add("dropdown");
    dropdownM.classList.add("dropdown-menu");
}

function myFunction() {
    var element = document.getElementById("navBar");
    var darkToggle = document.getElementById("darkToggle");
    const texts = document.getElementsByClassName("text-to-dark");
    const brand = document.querySelector('.navbar-brand');
    const dropdown = document.getElementsByClassName('dropdown-menu');
    var card = document.getElementById("card");
    var body = document.getElementById("body");
    var logoText = document.getElementById("logoText");
    var logoText1 = document.getElementById("logoText1");


    if (darkToggle.checked || darkToggle1.checked) {
        element.classList.add("navbar-dark");
        element.classList.remove("navbar-light");

        element.classList.add("bg-dark");

        for (let i = 0; i < dropdown.length; i++) {
            dropdown[i].classList.add("bg-dark");
        }

        element.classList.remove("bg-white");

        for (let i = 0; i < texts.length; i++) {
            texts[i].style.color = 'white';
        }

        brand.style.width = '100px';

        logoText.style.fill = 'white';
        logoText1.style.fill = 'white';

        card.style.background = '#515151';

        body.classList.add("bg-dark");

    } else {
        element.classList.add("navbar-light");
        element.classList.remove("navbar-dark");
        element.classList.remove("bg-dark");

        for (let i = 0; i < dropdown.length; i++) {
            dropdown[i].classList.remove("bg-dark");
        }

        element.classList.add("bg-white");

        for (let i = 0; i < texts.length; i++) {
            texts[i].style.color = 'black';
        }

        logoText.style.fill = 'black';
        logoText1.style.fill = 'black';

        card.style.background = '#D9D9D9';

        body.classList.remove("bg-dark");
    }
}