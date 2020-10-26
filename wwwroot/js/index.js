function updateProjektLinks() {
	const kw = document.getElementById("Datum").value;
	const links = document.getElementsByClassName("projektLink");

	for (const link of links) {
		link.setAttribute("href", `/woche?kw=${kw}&Projektnummer=${link.innerHTML}`);
	}
}

updateProjektLinks();