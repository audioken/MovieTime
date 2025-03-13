document.addEventListener("DOMContentLoaded", function () {
    // Meny-toggle
    const menuToggle = document.querySelector(".menu-toggle");
    const navbar = document.querySelector("#navbar");
    menuToggle.addEventListener("click", function () {
        navbar.classList.toggle("show");
    });

    // Lägg till funktionalitet för alla "Läs mer"/"Läs mindre"-knappar
    document.querySelectorAll("[data-bs-toggle='collapse']").forEach(button => {
        const targetId = button.getAttribute("data-bs-target");
        const targetElement = document.querySelector(targetId);

        // Se till att dessa händelser endast påverkar recensionselementen
        if (targetElement && targetId.startsWith("#reviewText-")) {
            targetElement.addEventListener("show.bs.collapse", function () {
                button.textContent = "Läs mindre..";
            });

            targetElement.addEventListener("hide.bs.collapse", function () {
                button.textContent = "Läs mer..";
            });
        }
    });

    // Skrolla till recensionerna om TempData säger till
    if (window.scrollToReviews) {
        const reviewsTitle = document.getElementById("reviews-title");
        if (reviewsTitle) {
            reviewsTitle.scrollIntoView();
        }
    }
});

