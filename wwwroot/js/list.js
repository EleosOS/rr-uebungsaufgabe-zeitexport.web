const p = document.querySelectorAll("p");

for (const element of p) {
	if (element.classList.contains("notiz") || element.classList.contains("stunden")) {
		element.onclick = () => {
			document.execCommand("copy");
			element.classList.add("active");

			setTimeout(() => {
				element.classList.remove("active");
			}, 2000);
		}
	
		element.addEventListener("copy", (event) => {
			event.preventDefault();
			if (event.clipboardData) {
				event.clipboardData.setData("text/plain", element.textContent);
				console.log(event.clipboardData.getData("text"));
			}
		});
	}
}