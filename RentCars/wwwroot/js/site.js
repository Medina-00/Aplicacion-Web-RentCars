


const burger = document.querySelector('.burger');
const navLinks = document.querySelector('.nav-links');

burger.addEventListener('click', () => {
    navLinks.classList.toggle('active');
    burger.classList.toggle('open');
});

window.onload = function () {
    var errorMessage = '@Model.Error';  // Asegúrate de que @Model.Error tiene el valor correcto
    var alertBox = document.querySelector('.alert');

    if (errorMessage && errorMessage.trim() !== '') {
        alertBox.style.display = 'block';
    }
}

// Función para deshabilitar las fechas pasadas
function setMinFechaInicio() {
    const fechaInicio = document.getElementById('fechaInicio');
    const today = new Date();
    const day = today.getDate().toString().padStart(2, '0');
    const month = (today.getMonth() + 1).toString().padStart(2, '0');
    const year = today.getFullYear();
    const minDate = `${year}-${month}-${day}`;
    fechaInicio.min = minDate; // Restringe las fechas pasadas
}

// Función para actualizar la fecha mínima de devolución según la fecha de recogida
function setMinFechaFin() {
    const fechaInicio = document.getElementById('fechaInicio').value;
    const fechaFin = document.getElementById('fechaFin');

    if (fechaInicio) {
        fechaFin.min = fechaInicio; // Establece la fecha mínima para FechaFin
    } else {
        fechaFin.min = ""; // Resetea si no hay fecha de inicio
    }
}

// Ejecutamos la función para que no se pueda elegir una fecha pasada para el inicio
setMinFechaInicio();