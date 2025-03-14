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

    // Lägg till funktion för att återställa filter
    document.getElementById("resetFilters").addEventListener("click", function () {
        // Återställ alla filter till sina ursprungliga värden
        document.getElementById("movieFilter").value = "";
        document.getElementById("userFilter").value = "";
        document.getElementById("ratingFilter").value = "";

        // Uppdatera tabellen så att alla recensioner visas igen
        filterTable();
    });

    function filterTable() {
        var movieFilter = document.getElementById("movieFilter").value.toLowerCase();
        var userFilter = document.getElementById("userFilter").value.toLowerCase();
        var ratingFilter = document.getElementById("ratingFilter").value;
        var rows = document.querySelectorAll(".review-row");

        rows.forEach(row => {
            var movie = row.getAttribute("data-movie").toLowerCase();
            var user = row.getAttribute("data-user").toLowerCase();
            var rating = parseFloat(row.getAttribute("data-rating")); // Konvertera rating till float

            // Om ratingFilter är ett nummer (inte tomt), jämför med rating
            var movieMatch = (movieFilter === "" || movie === movieFilter);
            var userMatch = (userFilter === "" || user === userFilter);
            var ratingMatch = (ratingFilter === "" || rating === parseFloat(ratingFilter)); // Exakt betyg

            // Visa eller dölj raden baserat på alla filter
            row.style.display = (movieMatch && userMatch && ratingMatch) ? "" : "none";
        });
    }

    // Lägg till event-listeners på filtren
    document.getElementById("movieFilter").addEventListener("change", filterTable);
    document.getElementById("userFilter").addEventListener("change", filterTable);
    document.getElementById("ratingFilter").addEventListener("change", filterTable);


});


