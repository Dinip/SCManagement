﻿setTheme(localStorage.getItem("theme") || "light")
document.getElementById("Dropdown").addEventListener('click', function (event) {
    event.stopPropagation();
});

document.getElementById("Dropdown1").addEventListener('click', function (event) {
    event.stopPropagation();
});


function darkThemeToggle(event) {
    if (event.checked) {
        localStorage.setItem("theme", "dark")
        setTheme("dark")
        return
    }
    localStorage.setItem("theme", "light")
    setTheme("light")
}

function setTheme(theme) {
    let element = document.getElementById("navBar");
    let darkToggle = document.getElementById("darkToggle");
    let darkToggle1 = document.getElementById("darkToggle1");
    let texts = document.getElementsByClassName('text-to-dark');
    let brand = document.querySelector('.navbar-brand');
    let dropdown = document.getElementsByClassName('dropdown-menu');
    let card = document.getElementsByClassName('card');
    let body = document.getElementsByClassName('body');
    let logoText = document.getElementById("logoText");
    let logoText1 = document.getElementById("logoText1");
    let modal = document.getElementsByClassName('modal-content');
    let dlBg = document.getElementsByClassName('dlBg');
    let noClubImg = document.getElementById('swapImage');
    if (theme === "dark") {
        darkToggle.checked = true
        darkToggle1.checked = true
        element.classList.add("navbar-dark");
        element.classList.remove("navbar-light");

        element.classList.add("bg-dark");
        if (noClubImg) {
            noClubImg.src = "../img/NoClubWhite.png";
        }
        for (let i = 0; i < dropdown.length; i++) {
            dropdown[i].classList.add("bg-dark");
        }

        element.classList.remove("bg-white");

        for (let i = 0; i < texts.length; i++) {
            texts[i].style.color = 'white';
        }

        brand.style.width = '158px';


        for (let i = 0; i < card.length; i++) {
            card[i].style.background = '#3d4245';
        }
        for (let i = 0; i < body.length; i++) {
            body[i].classList.add("bg-dark");
        }

        for (let i = 0; i < modal.length; i++) {
            modal[i].classList.add("bg-dark");
        }
        logoText.style.fill = 'white';
        if (logoText1) {
            logoText1.style.fill = 'white';
        }

        for (let i = 0; i < dlBg.length; i++) {
            dlBg[i].style.background = '#1a1a1a';
        }

    } else {
        darkToggle.checked = false
        darkToggle1.checked = false
        element.classList.add("navbar-light");
        element.classList.remove("navbar-dark");
        element.classList.remove("bg-dark");

        for (let i = 0; i < dropdown.length; i++) {
            dropdown[i].classList.remove("bg-dark");
        }
        if (noClubImg) {
            noClubImg.src = "../img/NoClubBlack.png";
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

        for (let i = 0; i < dlBg.length; i++) {
            dlBg[i].style.background = 'whitesmoke';
        }

        logoText.style.fill = 'black';
        if (logoText1) {
            logoText1.style.fill = 'black';
        }

        
    }
}