const card = document.querySelector('#card');
const logoCard = document.getElementById('logo-card');
const numberCard = document.querySelector('#card .number-card');
const nameCard = document.querySelector('#card .name-card');
const mounthExpirationCard = document.querySelector('#card .mounth-expiration-card');
const yearExpirationCard = document.querySelector('#card .year-expiration-card');
const firmCard = document.querySelector('#card .firm-card p');
const ccvCard = document.querySelector('#card .ccv-card');

const btnOpenForm = document.getElementById('btn-open-form-card');
const formCard = document.getElementById('form-card');
const numberCardForm = document.getElementById('number-card-form');
const nameCardForm = document.getElementById('name-card-form');
const selectMounthCardForm = document.getElementById('mounth-expiration-card-form');
const selectYearCardForm = document.getElementById('year-expiration-card-form');
const ccvCardForm = document.getElementById('ccv-card-form');

const showFrontCard = () => {
    if (card.classList.contains('active')) {
        card.classList.remove('active');
    }
}

card.addEventListener('click', () => {
    card.classList.toggle('active');
});

btnOpenForm.addEventListener('click', () => {
    btnOpenForm.classList.toggle('active');
    formCard.classList.toggle('active');
});

for (let i = 1; i <= 12; i++) {
    let option = document.createElement('option');
    option.value = i;
    option.innerText = i;
    selectMounthCardForm.appendChild(option);
}

let currentYear = new Date().getFullYear();

for (let i = currentYear; i <= currentYear + 8; i++) {
    let option = document.createElement('option');
    option.value = i;
    option.innerText = i;
    selectYearCardForm.appendChild(option);
}

numberCardForm.addEventListener('keyup', e => {
    let valueNumberCardForm = e.target.value
        .replace(/\s/g, '')
        .replace(/\D/g, '')
        .replace(/([0-9]{4})/g, '$1 ')
        .trim();
    numberCardForm.value = valueNumberCardForm;

    numberCard.textContent = valueNumberCardForm;

    if (valueNumberCardForm === '') {
        numberCard.textContent = '#### #### #### ####';
        logoCard.innerHTML = '';
    }

    if (valueNumberCardForm[0] === '4') {
        logoCard.innerHTML = '';
        let imgLogo = document.createElement('img');
        imgLogo.src = "/css/resource_card/visa.png";
        logoCard.appendChild(imgLogo);
    } else if (valueNumberCardForm[0] === '5') {
        logoCard.innerHTML = '';
        let imgLogo = document.createElement('img');
        imgLogo.src = "/css/resource_card/mastercard.png";
        logoCard.appendChild(imgLogo);
    } else if (valueNumberCardForm[0] === '3' && valueNumberCardForm[1] === '4' || valueNumberCardForm[0] === '3' && valueNumberCardForm[1] === '7') {
        logoCard.innerHTML = '';
        let imgLogo = document.createElement('img');
        imgLogo.src = "/css/resource_card/amex.png";
        logoCard.appendChild(imgLogo);
    } else if (valueNumberCardForm[0] === '6' && valueNumberCardForm[1] === '0' && valueNumberCardForm[2] === '1' && valueNumberCardForm[3] === '1') {
        logoCard.innerHTML = '';
        let imgLogo = document.createElement('img');
        imgLogo.src = "/css/resource_card/discover.png";
        logoCard.appendChild(imgLogo);
    } else if (valueNumberCardForm[0] === '9' && valueNumberCardForm[1] === '7' && valueNumberCardForm[2] === '9' && valueNumberCardForm[3] === '2') {
        logoCard.innerHTML = '';
        let imgLogo = document.createElement('img');
        imgLogo.src = "/css/resource_card/troy.png";
        logoCard.appendChild(imgLogo);
    } 

    showFrontCard();

});

nameCardForm.addEventListener('keyup', e => {
    let valueNameCardForm = e.target.value.replace(/[0-9]/g, '');

    nameCardForm.value = valueNameCardForm;
    nameCard.textContent = valueNameCardForm;
    firmCard.textContent = valueNameCardForm;

    if (valueNameCardForm === '') {
        nameCard.textContent = 'John Doe';
    }

    showFrontCard();
});


selectMounthCardForm.addEventListener('change', e => {
    mounthExpirationCard.textContent = e.target.value;
    showFrontCard();
});


selectYearCardForm.addEventListener('change', e => {
    yearExpirationCard.textContent = e.target.value.slice(2);
    showFrontCard();
});

ccvCardForm.addEventListener('keyup', e => {
    if (!card.classList.contains('active')) {
        card.classList.add('active');
    }

    ccvCardForm.value = ccvCardForm.value
        .replace(/\s/g, '')
        .replace(/\D/g, '');

    ccvCard.textContent = ccvCardForm.value;
});

var bg = [Math.floor(Math.random() * 25) + 1];
document.getElementsByClassName('front-card')[0].style.backgroundImage = 'url(/css/resource_card/' + bg.toString() + ".jpeg" + ')';
document.getElementsByClassName('back-card')[0].style.backgroundImage = 'url(/css/resource_card/' + bg.toString() + ".jpeg" + ')';

const regex = /[a-zA-Z\s]+$/;
function validate(e) {
    const chars = e.target.value.split('');
    const char = chars.pop();
    if (!regex.test(char)) {
        e.target.value = chars.join('');
    }
}
document.querySelector('#name-card-form').addEventListener('input', validate);
