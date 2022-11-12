# Funkční specifikace zápočtového programu

**Autor:** Milan Veselý

**Předmět:** NPRG035 a NPRG038

**Téma**: Online verze karetní hry Oko bere

---

## Popis programu

Cílem mého zápočtového programu je vytvořit dva programy - aplikace na straně serveru a klientská aplikace, které  umožní hrát karetní hru Oko bere a to proti počítači nebo ve více hráčích.

### Server aplikace

Umožňovala by propojit více hráčů k jednomu stolu (a tedy i stoly vytvářet a upravovat). Zároveň by tedy musela držet základní údaje o uživatelích (především množství jejich virtuální měny) a informace o právě probíhající hrách více hráčů (stav banku, hodnoty karet, ...). Zároveň by umožnila ke stolu připojit AI hráče (především bankéře).

### Klientská aplikace

Jednalo by se v první řadě o grafické rozhraní, které by umožnilo samotnou hru a to buď v single player módu nebo online módu. V single player módu by proti hráči hrál počítač a v online módu by se po zadání IP adresy a přezdívky (případně hesla) připojil k serveru s výběrem stolů.

---

### Pravidla hry oko bere

*Pravidla byly lehce zkráceny*

Oko bere je tradiční česká hra velmi podobná známější hře black jack. Hraje se standardním  balíčkem 32 karet o hodnotách 7, 8, 9, 10, kluk, dáma, král a eso a to ve čtyřech barvách. 

Hráči hry se dělí na dvě role - bankéř a hráč. Kde bankéř je zvolen na začátku hry náhodným losováním. Cílem hry je pak pro obě role získat hodnotu karet 21 nebo se alespoň co nejvíce přiblížit a nepřetáhnout. Přičemž karty kluk, dáma a král mají hodnotu jedna, karty s číslem mají jako hodnotu toho čísla a eso má hodnotu 11.

#### Speciální pravidla

* Pět karet v hodnotě jedna nebo karty v celkové hodnotě 15 se mohu vyměnit za novou kartu (jednou za kolo)

* Srdcová sedmička (*šantala*) může být buď 7, 10 nebo 11 (podle toho, co hráčovi nejvíce pomůže)

* Kombinace dvou es nebo šantala je tzv. královské oko a hráč automaticky vyhrává

* ...

#### Průběh hry

Nejdříve každý hráč včetně bankéře obdrží jednu kartu a následně hru začíná hráč nalevo od bankéře, ten má opakovaně možnost požádat o kartu další (se vsazením nebo bez) a nebo ukončit tah. Pokud hodnota jeho karet během tahu přesáhne 21, musí karty zahodit a ztrácí sázku do banku. Po skončení tahu následuje další hráč. Až přijde řada na bankéře, všichni hráči mají vsazeno a nebo karty zahodily. Bankéř si bere karty (ale už nevsází), až se rozhodne, že má dost, otočí karty a každý, kdo má více než on obdrží dvojnásobek svojí sázky, v opačném případě jeho sázka putuje do banku.

Hra může skončit tak, že bankéř zvolá "malá domů" a je mu povoleno skončit pouze v případě, že po následujícím rozdání obdrží krále nebo osmičku (v opačném případě se pokračuje dále).

---

### Použité technologie z hlediska splnění NPRG038

Nejzásadněji by bylo využito síťování, přičemž obě aplikace alespoň nějak zásadně budou využívat více vláken.
