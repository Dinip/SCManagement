
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

function editProfile() {
    var profile = document.getElementById("cardBodyProfile");
    var edit = document.getElementById("cardBodyProfileEdit");
    profile.classList.add("d-none");
    edit.classList.remove("d-none");
}

function saveProfile() {
    var profile = document.getElementById("cardBodyProfile");
    var edit = document.getElementById("cardBodyProfileEdit");
    profile.classList.remove("d-none");
    edit.classList.add("d-none");
}

function myFunction() {
    var element = document.getElementById("navBar");
    var darkToggle = document.getElementById("darkToggle");
    const texts = document.getElementsByClassName('text-to-dark');
    const brand = document.querySelector('.navbar-brand');
    const dropdown = document.getElementsByClassName('dropdown-menu');
    const card = document.getElementsByClassName('card-to-dark');
    const body = document.getElementsByClassName('body');
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

        for (let i = 0; i < card.length; i++) {
            card[i].style.background = '#515151';
        }
        for (let i = 0; i < body.length; i++) {
            body[i].classList.add("bg-dark");
        }

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

        for (let i = 0; i < card.length; i++) {
            card[i].style.background = '#D9D9D9';
        }

        for (let i = 0; i < body.length; i++) {
            body[i].classList.remove("bg-dark");
        }
    }
}