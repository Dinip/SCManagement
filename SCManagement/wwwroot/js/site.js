
document.getElementById("Dropdown").addEventListener('click', function (event) {
    event.stopPropagation();
});

document.getElementById("Dropdown1").addEventListener('click', function (event) {
    event.stopPropagation();
});

const dropdown2 = document.querySelector('.dropdown');
const dropdownM = document.querySelector('.dropdown-menu');
var elem = document.getElementById('dropdownMenuButton1');
var fatherElem = elem.parentNode;
if (window.innerWidth <= 768) {
    fatherElem.removeChild(elem);

    dropdown2.classList.remove("dropdown");
    dropdownM.classList.remove("dropdown-menu");
} else {
    dropdown2.classList.add("dropdown");
    dropdownM.classList.add("dropdown-menu");
}

function myFunction() {
    let element = document.getElementById("navBar");
    let darkToggle = document.getElementById("darkToggle");
    let texts = document.getElementsByClassName('text-to-dark');
    let brand = document.querySelector('.navbar-brand');
    let dropdown = document.getElementsByClassName('dropdown-menu');
    let card = document.getElementsByClassName('card');
    let body = document.getElementsByClassName('body');
    let logoText = document.getElementById("logoText");
    let logoText1 = document.getElementById("logoText1");
    let modal = document.getElementsByClassName('modal-content');
    let cardcolor;
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

        brand.style.width = '158px';

        
        for (let i = 0; i < card.length; i++) { 
            card[i].style.background = '#515151';
        }
        for (let i = 0; i < body.length; i++) {
            body[i].classList.add("bg-dark");
        }

        for (let i = 0; i < modal.length; i++) {
            modal[i].classList.add("bg-dark");
        }
        logoText.style.fill = 'white';
        logoText1.style.fill = 'white';
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

        

        for (let i = 0; i < card.length; i++) {
            card[i].style.background = '#D9D9D9';
        }

        for (let i = 0; i < body.length; i++) {
            body[i].classList.remove("bg-dark");
        }

        for (let i = 0; i < modal.length; i++) {
            modal[i].classList.remove("bg-dark");
        }

        logoText.style.fill = 'black';
        logoText1.style.fill = 'black';
    }
}