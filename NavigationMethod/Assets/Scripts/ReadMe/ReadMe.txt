SOURCE: https://www.youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW

---------------
|             |
| G         H |
|             |
|      F      |
|             |
---------------

G cost => baslangic ile node arasindaki mesafe
H cost => (Heuristic) node ile bitis arasindaki kus ucusu mesafe
F cost = G + H

-> Baslangic gridin komsu gridleri open listesine eklenir.

-> Open listesinde en dusuk F degeri olani secilir.
    -> Eger F degeri en dusuk olan birden fazla varsa H degeri en kucuk olan secilerek devam edilir.
        -> Eger H degeri en dusuk olan da birden fazla ise random bir tane secilir.
            -> Her secilen makul grid parent i ile iliskilendirilir.

-> Kontrol edilen gridler close listesine eklenir.

-> Bir sonraki komsu grid kontrol edilir.
    -> Eger komsu grid hedef grid ise hedefe olasilmistir.
        -> Bulunan yoldan gridler parentleri ile tersten gidilerek yol cizilmis olur.
    -> Eger open listesinde grid kalmadıysa yol yoktur.