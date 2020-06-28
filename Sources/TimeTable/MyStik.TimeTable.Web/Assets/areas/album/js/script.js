// ============== automatische Slideshow ==============

// wie lange bleibt ein Slide stehen -- Sekunden
const standDauer = 5;

// Variable legt fest, ob Slideshow automatisch abläuft oder nicht
// hier erst mal pausiert, bis User auf "Space"-Taste oder "Play"-Symbol drückt
let slideShowLaufen = false;

// globale Variable, die speichert, auf welches kleines Bild
// man gerade geklickt hat
// wird für das Weiter- und Zurückgehen bei den Grossbildern benötigt
let kleinBildID;

// ===================== eingebaute KeyEvents =====================

/*

  ArrowDown, ArrowRight:    Grossbild eins weiter (nur wenn GrossBild sichtbar)
  ArrowUp, ArrowLeft:       Grossbild eins zurück (nur wenn GrossBild sichtbar)
  Escape, Backspace:        Grossbild-Ansicht und Slideshow beenden
  SPACE (Leertaste):        automatische Slideshow starten/beenden (nur wenn GrossBild sichtbar)

*/

// ================ Größe des Browserfensters abfragen ================

// wenn das Browserfenster klein ist,
// wird statt dem großen Bild das kleine (im Ordner "s")
// bzw. das mittlere (im Ordner "m") in das Popup und die Slideshow geladen

let zielOrdner = "b";
function browserGroesse() {
  if (window.innerWidth < 650 || window.innerHeight < 400) {
    zielOrdner = "s";
  }

  if (
    (window.innerWidth >= 650 && window.innerWidth < 1000) ||
    (window.innerHeigh >= 400 && window.innerHeight < 700)
  ) {
    zielOrdner = "m";
  }
}

// fragt die Browsergröße beim Laden der Seite ab
browserGroesse();
// console.log(`zielOrdner: ${zielOrdner}`);

// =============== erst Bilder zeigen, wenn alle geladen sind ===============

// DEAKTIVIERT, weil Performance des Raspi dafür nicht gut ist!
const loading = document.querySelector("#loading");

// der "Loading"-Text ist bei aktivem window.addEventListener("load")
// eigentlich zuerst sichtbar, aber da das ja aktuell deaktiviert ist
// hier auch rausgenommen!
loading.style.display = "none";

const galerie = document.querySelector(".galerie");

/*
Performance-Probleme bei schlechter Internet-Verbindung
und/oder Smartphones, deswegen rausgenommen
*/
/*
window.addEventListener("load", function() {
  loading.style.opacity = "0";
  galerie.classList.add("sichtbar");
});
*/

// ============= Elemente für GrossBild-Sachen vorab generieren =====================

const abdecker = document.createElement("div");
abdecker.id = "abdecker";

// die animierte "Sanduhr", die anzeigt, dass ein großes Bild geladen wird
const abdeckerSanduhr = document.createElement("img");
abdeckerSanduhr.id = "abdeckerSanduhr";
abdeckerSanduhr.src = "/Assets/areas/album/p/waiting_white.svg";

const grossBild = document.createElement("figure");
grossBild.id = "grossBild";

// 17.4.20 neu
const grossBildDiv = document.createElement("div");

const grossBildImg = document.createElement("img");
const grossBildFigcaption = document.createElement("figcaption");

const schliesser = document.createElement("span");
schliesser.id = "schliesser";
schliesser.textContent = "×";
schliesser.role = "button";
schliesser.tabIndex = "0"; // Schreibweise in JS beachten!
schliesser.title = "Großbild schliessen";

const weiterSymbol = document.createElement("img");
weiterSymbol.src = "/Assets/areas/album/p/forward.svg";
weiterSymbol.id = "weiterSymbol";
weiterSymbol.role = "button";
weiterSymbol.tabIndex = "0";
weiterSymbol.title = "nächstes Bild zeigen";

const zurueckSymbol = document.createElement("img");
zurueckSymbol.src = "/Assets/areas/album/p/back.svg";
zurueckSymbol.id = "zurueckSymbol";
zurueckSymbol.role = "button";
zurueckSymbol.tabIndex = "0";
zurueckSymbol.title = "vorheriges Bild zeigen";

const playSymbol = document.createElement("img");
playSymbol.src = "/Assets/areas/album/p/play.svg";
playSymbol.id = "playSymbol";
playSymbol.role = "button";
playSymbol.tabIndex = "0";
playSymbol.title = "automatische Slideshow starten/anhalten";

// zusammenbauen und einfügen
grossBildDiv.appendChild(grossBildImg); // 17.4.20 neu
grossBild.appendChild(grossBildDiv); // 17.4.20 neu
grossBild.appendChild(grossBildFigcaption);
grossBild.appendChild(schliesser);
grossBild.appendChild(weiterSymbol);
grossBild.appendChild(zurueckSymbol);
grossBild.appendChild(playSymbol);
abdecker.appendChild(abdeckerSanduhr);
abdecker.appendChild(grossBild);
document.body.appendChild(abdecker);

// ============== Ende der Generierung ===============

function grossBildZeigen(ereignis) {
  ereignis.preventDefault();

  let myTitle = ereignis.currentTarget.title;
  let myLink = ereignis.currentTarget.href;

  // zielOrdner abhängig von Browser-Größe
  // wird beim Laden der Seite abgefragt
  myTitle = myTitle.replace("/b/", `/${zielOrdner}/`);
  myLink = myLink.replace("/b/", `/${zielOrdner}/`);

  // zum Zeigen des nächsten bzw. vorherigen Grossbildes
  kleinBildID = ereignis.currentTarget.id;
  // console.log(`ID des gerade angeklickten Links: ${kleinBildID}`);

  grossBildImg.src = myLink;
  grossBildImg.alt = myTitle;
  grossBildFigcaption.textContent = myTitle;

  abdeckerSanduhr.classList.add("sichtbar");
  abdecker.classList.add("sichtbar");

  abdecker.addEventListener(
    "transitionend",
    function() {
      // console.log("Abdecker-Transition beended!");
      grossBild.classList.add("sichtbar");
    },
    { once: true } // damit läuft dieser "addEventListener" nur einmal und addiert sich nicht auf
  );
}

function grossBildWeg() {
  abdeckerSanduhr.classList.remove("sichtbar");
  grossBild.classList.remove("sichtbar");

  // Slideshow soll nicht weiterlaufen
  slideShowLaufen = false;
    playSymbol.src = "/Assets/areas/album/p/play.svg";

  grossBild.addEventListener(
    "transitionend",
    function() {
      // console.log("grossBild-Transition beended!");
      abdecker.classList.remove("sichtbar");

      // "leeres" Bild - damit nicht beim nächsten "grossBildZeigen"
      // noch teilweise das alte Bild gezeigt wird und dann während
      // der Transition ausgetauscht wird - das stört sehr!
      // besonders merkbar, wenn die Bilder vom Server geladen werden
      // und eine langsame Internet-Verbindung besteht
      grossBildImg.src = "";
    },
    { once: true }
  );
}

// die Links, in denen sich die Bilder der Galerie befinden

const myImageLinks = document.querySelectorAll(".galerie a");
console.log("register events for: " + myImageLinks);
myImageLinks.forEach(function(ereignis) {
  ereignis.addEventListener("click", grossBildZeigen);
});


// "×" zum Schliessen des grossBild
schliesser.addEventListener("click", grossBildWeg);
abdecker.addEventListener("click", grossBildWeg);

// ================ Weiterklicken bei sichtbaren Großbild ================

function grossBildTausch(id) {
  // console.log(ele);

  grossBild.classList.remove("sichtbar");

  grossBild.addEventListener(
    "transitionend",
    function() {
      // console.log("Abdecker-Transition beended!");
      grossBildImg.src = myImageLinksArray[id].href;
      grossBildFigcaption.textContent = myImageLinksArray[id].title;

      // Bildgröße von Größe des Browserfenster abhängig
      // zielOrdner wird beim Laden der Seite abgefragt
      grossBildImg.src = grossBildImg.src.replace("/b/", `/${zielOrdner}/`);
      grossBildFigcaption.textContent = grossBildFigcaption.textContent.replace(
        "/b/",
        `/${zielOrdner}/`
      );
      // myLink = myLink.replace("/b/", `/${zielOrdner}/`);
      grossBild.classList.add("sichtbar");
    },
    { once: true } // damit läuft dieser "addEventListener" nur einmal und addiert sich nicht auf
  );
}

function naechstesBildZeigen(ereignis) {
  // sonst würde das Klicken zum "abdecker" durchgereicht,
  // der das Grossbild ausblendet…
  ereignis.stopPropagation();

  // jeweils einen Eintrag im Array weitergehen
  if (kleinBildID < myImageLinksArray.length - 1) {
    kleinBildID = parseInt(kleinBildID) + 1;
  } else {
    kleinBildID = 0;
  }

  grossBildTausch(kleinBildID);
}

function vorherigesBildZeigen(ereignis) {
  // sonst würde das Klicken zum "abdecker" durchgereicht,
  // der das Grossbild ausblendet…
  ereignis.stopPropagation();

  // jeweils einen Eintrag im Array weitergehen
  if (kleinBildID > 0) {
    kleinBildID = parseInt(kleinBildID) - 1;
  } else {
    kleinBildID = myImageLinksArray.length - 1;
  }

  grossBildTausch(kleinBildID);
}

// in ein Array umwandeln
const myImageLinksArray = Array.from(myImageLinks);

for (let i = 0; i < myImageLinksArray.length; i++) {
  // die ID des jeweiligen Links wird hier automatisch erzeugt
  // ACHTUNG: das ist nur eine Ziffer (0, 1, 2 …)! Eigentlich bei IDs nicht erlaubt!
  // es gab aber noch keine Probleme…
  const meinID = `${i}`;

  // und im HTML eingefügt
  myImageLinksArray[i].id = meinID;
}

// =============== selbstablaufende Slideshow ================

// lässt die Bilder hintereinander ablaufen
function weiterGehen() {
  // jeweils einen Eintrag im Array weitergehen
  if (kleinBildID < myImageLinksArray.length - 1) {
    kleinBildID = parseInt(kleinBildID) + 1;
  } else {
    kleinBildID = 0;
  }

  grossBildTausch(kleinBildID);
}

// startet/stoppt die Slideshow,
// ausgelöst durch "Space"-Taste oder playSymbol zum Anklicken
function slideShowAnAus() {
  slideShowLaufen = !slideShowLaufen;
  if (slideShowLaufen === true) {
      playSymbol.src = "/Assets/areas/album/p/pause.svg";
  } else {
      playSymbol.src = "/Assets/areas/album/p/play.svg";
  }
}

// wenn die Variable "slideShowLaufen" "true" ist, läuft die Slideshow
const cycle = setInterval(function() {
  if (slideShowLaufen == true) {
    weiterGehen();
  }
}, standDauer * 1000);

// ===================== KeyEvents =====================

function handleKey(ereignis) {
  // KeyEvents nur, wenn "abdecker" (schwarze Fläche) sichtbar ist
  if (abdecker.classList.contains("sichtbar")) {
    // console.log(ereignis.key);

    ereignis.preventDefault();

    // weiterblättern
    if (ereignis.key == "ArrowRight" || ereignis.key == "ArrowDown") {
      naechstesBildZeigen(ereignis);
    }

    // zurückblättern
    if (ereignis.key == "ArrowLeft" || ereignis.key == "ArrowUp") {
      vorherigesBildZeigen(ereignis);
    }

    // Bildershow beenden
    if (ereignis.key == "Escape" || ereignis.key == "Backspace") {
      grossBildWeg();
      // Slideshow soll dann natürlich nicht mehr laufen
      slideShowLaufen = false;
        playSymbol.src = "/Assets/areas/album/p/play.svg";
    }

    // automatische Slideshow starten/anhalten bei Drücken der "Space"-Taste
    if (ereignis.key == " ") {
      slideShowAnAus();
    }
  }
}

window.addEventListener("keydown", handleKey);

const weiterButton = document.querySelector("#weiterSymbol");
weiterButton.addEventListener("click", naechstesBildZeigen);

const zurueckButton = document.querySelector("#zurueckSymbol");
zurueckButton.addEventListener("click", vorherigesBildZeigen);

const playButton = document.querySelector("#playSymbol");
playButton.addEventListener("click", function(ereignis) {
  ereignis.stopPropagation();
  slideShowAnAus();
});

// ============== Figcaption-Text in der Galerie-Ansicht kürzen ==============

const galerieFigcaptions = document.querySelectorAll(".galerie figcaption");

galerieFigcaptions.forEach(function(ereignis) {
  const laenge = 120; // so viele Zeichen darstellen
  let langText = ereignis.textContent;
  let kurzText = langText.substring(0, laenge) + "…";
  ereignis.textContent = kurzText;
});

// ======== Pulldown oberhalb der Galerie mit weiteren BewerberInnen ========
/*
const showPullDown = document.querySelector("#showPullDown");
const hidePullDown = document.querySelector("#hidePullDown");
const pullDown = document.querySelector("#pullDown");

showPullDown.addEventListener("click", function() {
  pullDown.classList.add("sichtbar");
});

hidePullDown.addEventListener("click", function() {
  pullDown.classList.remove("sichtbar");
});
*/

// die Buttons
const cardTextFormatieren = document.querySelectorAll(".cardTextFormatieren");

function preformatiertTogglen(ereignis) {
    const myCardText = ereignis.target.previousElementSibling;
    myCardText.classList.toggle("preformatiert");
}

// der jeweilige Button formatiert den darüber befindlichen "div.card-text"
// ACHTUNG: dafür muss in der CSS-Datei folgendes eingefügt sein:

/*
.preformatiert {
  white-space: pre;
  font-family: inherit;
}
*/

// und in HTML natürlich die Buttons mit der Klasse "cardTextFormatieren"

cardTextFormatieren.forEach(function (ereignis) {
    ereignis.addEventListener("click", preformatiertTogglen);
});
