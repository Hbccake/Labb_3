# Labb_3


Viktigaste tekniska lösningen var att jag valde att använda enum när jag gjorde dialogrutorna, så att jag inte behövde göra flera metoder för att sköta öppnandet av vald dialogruta. Det underlättade även enormt när jag skulle lägga till ytterligare en dialogruta för att importera från TriviaDb.
Det hjälpte också när jag började göra flera metoder för att switcha views och jag insåg att jag borde kunna göra samma sak där för att hoppa mellan views.

Det var trixigt att få till Json och importen av TriviaDb frågor, där testade jag ganska mycket fram och tillbaks med tips från google tills jag hittade en kombination som funkade.
och jag fick lite problem när jag importerade frågor från TriviaDB att vissa specialtecken inte importerades korrekt, men det gick ganska snabbt att klura ut med hjälp av API dokumentationen och lite googlande på hur man avkodar text.
(Väldigt roligt att få exprimentera med Json och import från nätet vill jag bara tillägga)

Men det största problemet jag stött på är min timer. När den körs så vill den hoppa över siffror i UI. Så jag trodde först att det var fel på min countdown, men efter flera olika varianter av tidtagning började jag fundera på om problemet var någon annanstans. Och efter intensivt googlande fick fram att det kan bli problem med att UI tråden blockerades, att stopwatch räknade ner korrekt men dispatchtimer hänger inte med och uppdaterar UI korrekt. 
Eftersom jag inte visste hur jag skulle kontrollera och tydligt analysera var problemet låg, så frågade jag copilot om min UI tråd blockerades. Han skickade mig till ett analysverktyg så jag körde analysen på min CPU. Och den analysen sa att det inte fanns något som blockerade tråden. Så tekniskt sett har jag faktiskt inte löst det problemet. (Ironiskt nog såg jag att UI uppdaterade korrekt på vissa frågor när jag körde analysen.) Så antingen kommer du se en korrekt räknande timer när du kör, eller så kanske du ser direkt var problemet ligger.
