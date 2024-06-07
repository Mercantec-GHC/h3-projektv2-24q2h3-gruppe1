### H3-Smart-Kommunikationsplatform - Water my pot

- Dokumentation
	- setup.md
	- bruger_manual.md
	-- Flowcharts
	-- ER diragrammer
	- Kravsspecificationer.md
	- Case_beskrivelse.md
- Arduino
	-- Biblioteker
	-- Sketches
- Database
- Backend (api)
- Frontend
- Tests
.gitignore
README.md

hardware:
2 motroer 
1 breadboard
1 arduino opla
1 carrier 
2 moisture sensor 
1 batteri

arduino opsætning link
https://edumercantec-my.sharepoint.com/:w:/g/personal/kasp728h_edu_mercantec_dk/ER_L0DI7PcxKqu4w2yCBPpoBz_kl4gHjbhax_bbsrdaxVg?e=fvy0Ci
### Projekt pitch:

"Vi vil lave et vandingssystem, som man både kan benytte manuelt, men også automatisk. Hvor brugeren vælger hvilken plante som er tilknyttet hvilken sensor, så vil en ventil åbne og kunne vande planterne, som gør brug af disse hardwarekomponenter: arduino 2+ moisture, heat sensor and a motor. Vores mål er at udvikle en løsning, der kan vande planter, ved at integrere et interaktivt system, der kan indsamle og reagere på data. For at opnå dette, tænker vi at anvende  programmeringssprog c#(blazor og asp .net api), databaser mysql(heidi). til at opbygge systemet, som vil tillade os at bruge teknologien, til hvis man nu glemmer at vande dine planter, at det bliver gjort automatisk når fugtigheden er for lav, at den så selv vil vande planten, hvor vi har motor som åbner og lukker en ventil, som er forbundet til en vand container.

Vores system vil kunne interagere med brugerne gennem et dashboard, der viser 1+ graf og hvor man kan vælge dage og man skal kunne tilføje en ny plante. om den skal være i auto eller manuel tilstand og hvor meget vand der er i containeren], hvilket giver brugerne mulighed for at  vælge en plante fra en drop down eller de kan selv skrive en plante ind og hvor meget vand den skal bruge. De kan se en graf over fugtigheden og rumtemperaturen.

