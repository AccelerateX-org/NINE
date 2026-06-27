---
sidebar_position: 2
---

# Module

Das Lehrangebot einer Institution ist in Module untergegliedert, wobei Module in Modulkatalogen organisiert sind. Ein Modul ist dabei immer eindeutig einem Modulkatalog zugeordnet. Ein Modul selbst kann in Teilmodule (=Fächer) unterteilt werden. Darüber hinaus können auf Ebene eines Moduls Prüfungsangebote definiert werden.

Hinweis:
Die Zuordnung von Modulen zu Studienprogrammen erfolgt durch "Akkreditierung".

Die Pflege der Modulkataloge und Module sowie Fächer erfolgt auf Ebene der Einrichtung. 

## Modulkataloge

Jede Einrichtung kann beliebig viele Modulkataloge anlegen. Ein Modulkatalog ist gekennzeichnet durch eine innerhalb der Einrichtung eindeutigen Kurzname und eine Bezeichnung. Ein Modulkatalog setzt sich aus Modulen zusammen.

Hinweis:
Die Systematik der Modulkataloge kann frei gewählt werden, z.B. entlang von Fachgruppen oder Studienprogrammen.

Zum Anlegen von Modulkatalogen erfordern administrative Rechte auf Ebene von Studiengängen. Für einen Modulkatalog können Mitglieder der eigene Einrichtung als Katalogverantwortliche festgelegt werden. Katalogverantwortliche können weitere Katalogverantwortliche aufnehmen oder löschen. Katalogverantwortliche haben das Recht Module anzulegen und zu verändern.

## Module

### Struktur

Als Elemente eines Moduls gelten
- Titel (de/en)
- Lehr- / Lernziele
- Fächer
- Lehrformate
- konkretes Prüfungsangebot
  - Auswahl aus der Liste der Prüfungsformate des Moduls
  - Zugelassene Hilfsmittel
  - Beschreibung der Anforderungen, Ablauf (v.a. bei ModA)

Eine Änderung des Modultitels geht in der Regel einher mit der Änderung der Lehr- / Lernziele. Umgekehrt führt eine Änderung der Lehr- / Lernziele nicht zwingend zu einer Änderung des Modultitels.

:::info
Überlegung: was würde passieren, wenn der Modultitel zur semesterweisen Fortschreibung gehört. Dann hätte ein Modul keinen zeitlich invarianten Titel (wie der Modulslot), sondern kann semester für semester anders heißen, mit anderen Inhalten.

Der Bedarf zum eigenen Titel kommt aus dem der Zuordnung eines Moduls zu einem Studienangebot, wobei ja das Fach angebunden wird, welches einen Titel hat. Mit anderen Worten: es wird unterschieden zwischen der Domäne der Module und deren Einsatz in der Domäne Studienangebote. Diese Trennung erzeugt zunächst Komplexität,liefert aber Spielräume zur Abbildung von real beobachten "Sonderfällen", wie "AW in SG A 2,5 LP, in SG B 3 LP".
:::


## Vorgehen / Anwendungsfälle

Grundidee: die Prozesse werden in der Anwendung ausgelöst und verfolgt.


### Änderung am Modul - Ebene Studienplan

Änderungen an Lehr- und Prüfungsangebot sowie Lehr-/Lernzielen für das Folgesemester möglich, bis zur Verabschiedung und Fortschreibung.

Kann vom MV gemacht werden.

### Änderungen am Modul - Ebene SPO

Eine Änderung ist nur dann möglich, wenn beide Bedingungen erfüllt sind (UND)

- Keines der Fächer eines Modul ist einem Studiengang zugeordnet ("akkreditiert")
- Es kein Lehrangebot zugeordnet

Kann vom MV gemacht werden

Alles andere führt zu einer neuen Version eines Moduls. Diese Kopie ist keinem Studiengang zugeordnet und daher änderbar.

Situation: Keine Akkreditierung, aber Lehrveranstaltungen

- Ohne Akkreditierung keine Abrechnung im LVN möglich
- Reine Spassveranstaltung
- Historisierung kann trotzdem sinnvoll sein

Moduländerung löst keine SPO-Änderung aus.

Situation: Akkreditierung, abe keine Lehrveranstaltungen

- "wurde bisher nie angeboten"
- kein bei "Pflichtmodulen" nicht passieren
- Annahme "Wahl(pflicht)modul"

Moduländerung löst SPO-Änderung aus.


### Änderungen an einer SPO - Modulebene

Es kommt zu einer Änderung an einem oder mehreren Modulen.

Es wird im aktuellen Studienangebot ein Change-Request angelegt

- Bisheriges Modul
- Neues Modul
- Status

### Änderung einer SPO - Strukturebene

Eine veröffentlichte SPO kann auf Strukturebene nicht geändert werden.

- keine Änderung an Slots
- keine Änderung an Themengebiete und Optionen

Es muss eine Kopie angelegt werden. Diese Kopie ist unveröffentlicht und damit änderbar.

Beim Kopieren können die Änderungsanträge berücksichtigt werden.

Eine neue SPO anlegen, dass kann nur wer? => Admin-Rechte auf Ebene Studienangebote => das reicht zunächst

Eine neue SPO hat keine Änderungsanträge, weil nicht veröffentlicht.

## Module (alt)

Ein Modul ist durch eine innerhalb des zugeordneten Modulkatalogs eindeutigen Kurznamen und eine Versionsnummer gekennzeichnet und besitzt einen Modultitel, jeweils in deutscher und englischer Sprache.

Anmerkungen:
Wird ein Modul verschoben, so muss ggf. die Kurzbezeichnung verändert werden. Die Kurzbezeichnung selbst hat nur nachrichtliche Bedeutung, z.B. zur Suche nach Modulen in Listen o.ä.
Beim Verschieben eines Moduls kann angegeben werden, in wie weit die Versionshistorie ebenfalls verschoben werden soll.

Ein Modul untergliedert sich in Fächer und Prüfungsangebote.

Hinweis:
Inhaltliche Aspekte, die sog. "Modulbeschreibung" wird hier nicht beschrieben.

### Fächer

Ein Fach kann einen Kurznamen und eine Bezeichnung haben. Es hat ein Lehrformat und einen Betreuungsaufwand gekennzeichnet durch Angabe von SWS.

Hinweis: 
Die SWS eines Fachs werden bei einer Abrechnung auf die Lehrenden verteilt, die eine zugehörige Lehrveranstaltung gehalten haben. Mit anderen Worten: eine Lehrveranstaltung "kostet" exakt die im Fach angegebenen SWS. 

Die verfügbaren Lehrformate werden auf Ebene der Institution verwaltet.

### Prüfungsform

Ein Modul kann ein oder mehrere Prüfungsangebote im Range einer "Modulprüfung" haben.

Ein Prüfungsform besteht aus einer oder mehrerer Teilprüfungen, wobei jede Teilprüfung folgende Angaben umfasst

- Prüfungsformat (verwaltet auf Ebene der Institution)
- Gewicht innerhalb der "Modulprüfung"

### Versionshistorie

Die strukturellen Angaben eines Moduls leiten sich in der Regel aus den zugehörigen Rechtsvorschriften, z.B. Studien- und Prüfungsordnungen ab. Diese legen in der Regel fest, dass jedwede Änderung einer Genehmigung durch die jeweiligen Gremien bedarf. Vor diesem Hintergrund sind Änderungen an Modulen nur möglich, so lange des Modul "nicht verwendet" wird. Als Verwendung gilt dabei

- Dem Modul sind bereits begonnene Lehrveranstaltungen zugeordnet.
- Das Modul ist einem verabschiedeten Studienprogramm zugeordnet.

Von einem Modul können Kopien erstellt werden. Die Kopie erhält die identische Struktur. Zuordnungen etc. werden nicht übernommen. Es wird ein eindeutiger Kurznamen generiert.

Hinweis:
Bei Fehlern etc, können Änderungen durch die "SuperAdmin" einer Einrichtung vorgenommen werden.

Wichtig:
Nicht verwechseln mit der Versionshistorie der zugehörigen Modulbeschreibungen.

## Modulbeschreibungen

Eine Modulbeschreibung beschreibt die konkrete operative Ausgestaltung eines Moduls in einer Lehrperiode im Sinne eines Studienplans. Dies umfasst:

- Beschreibung von Inhalten (nach Vorgabe der Akkreditierungsverordnung)
- Zugehörige Lehrveranstaltung(en)
- Prüfungsangebot bestehend aus
  - ausgewählter Prüfungsform ergänzt um
  - Angabe zu Prüfenden (Erst-, Zeitprüfende)
  - optional: Prüfungsdauer
  - optional: Zugelassene Hilfsmittel
  - optional: weitere Detailangaben

### Fortschreibung

Eine Fortschreibung der Modulbeschreibung erzeugt eine Kopie mit Zuordnung zur gewünschten Lehrperiode, in der Regel das Folgesemester.

Je nach geltenden Rechtsvorschriften sind Änderungen an Modulbeschreibungen auch nach Beginn einer Lehrperiode möglich. 

### Verabschiedung

Durch explizite Ausführung der Aktion "Verabschiedung" werden alle Modulbeschreibungen "eingefroren" und können nicht mehr verändert werden.