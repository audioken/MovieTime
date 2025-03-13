document.addEventListener("DOMContentLoaded", function () {
    const menuToggle = document.querySelector(".menu-toggle");
    const navbar = document.querySelector("#navbar");

    menuToggle.addEventListener("click", function () {
        navbar.classList.toggle("show");
    });
});

document.addEventListener("DOMContentLoaded", function () {
	var collapseElement = document.getElementById("reviewText-@review.Id");
	var button = document.querySelector(`[data-bs-target="#reviewText-@review.Id"]`);

	collapseElement.addEventListener("show.bs.collapse", function () {
		button.textContent = "Läs mindre..";
	});

	collapseElement.addEventListener("hide.bs.collapse", function () {
		button.textContent = "Läs mer..";
	});
});

document.addEventListener("DOMContentLoaded", function () {
    // För varje knapp med data-bs-toggle="collapse", sätt upp en event listener
    document.querySelectorAll("[data-bs-toggle='collapse']").forEach(button => {
        const targetId = button.getAttribute("data-bs-target");
        const targetElement = document.querySelector(targetId);

        // Lägg till event listeners för när kollapsen öppnas eller stängs
        targetElement.addEventListener("show.bs.collapse", function () {
            button.textContent = "Läs mindre.."; // Byt till "Läs mindre.." när texten visas
        });

        targetElement.addEventListener("hide.bs.collapse", function () {
            button.textContent = "Läs mer.."; // Byt tillbaka till "Läs mer.." när texten döljs
        });
    });
});

