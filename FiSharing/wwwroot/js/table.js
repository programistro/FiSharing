document.querySelectorAll('.toggle').forEach(button => {
    button.addEventListener('click', () => {
        const content = button.nextElementSibling;
        const isActive = content.classList.contains('active');

        content.classList.toggle('active');
        button.textContent = isActive ? '+' : '-';
    });
});