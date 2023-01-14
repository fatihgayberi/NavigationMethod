

https://user-images.githubusercontent.com/46620361/212444325-304fb92d-96c9-4ceb-9d03-daac9291201d.mp4




- Arazi üzerinde bulunan karakterlerin A* Star algoritması sayesinde bir noktadan başka bir noktaya en kısa yolu kullanarak gitmesini sağlayan demo.

- Arazide bulunan karakterler için tipler oluşturup belirli kıstaslar belirlenir ve bu kıstaslara göre hareket edebilmektedirler.

- Karakterler eğer belirlenen kıstaslara göre bir yol bulunmuyorsa hareket etmeden beklerler.

- Karakter tipleri oluşturup özelliklerini belirleyebiliyoruz;
~~~ _*NodeRadius:*_ Karakterin araziyi grid lere bölerken ne kadar küçük parçalara bölmesi
~~~ _*WorldSizeExpand:*_ Karakterin arazi içerisnde yol bulamazsa gridin genişleme miktarı
~~~ _*WorldSizeExpandMax:*_ Karakterlerin maksimum ne kadarlık bir grid içerisinde araziyi taraması gerektiği
~~~ _*DistTolerance:*_ Karkterin gidilecek noktaya olan uzaklığı minimum ne kadar ise o alana doğru ilerlemesi gerektiği
~~~ _*NewRotaWaitTime:*_ Karakterin hareket halinde iken yolda patrol olan bir obje ile karşılaşması dahilinde kaç saniye bekleyip yeniden rota çizmesi gerektiği
~~~ _*MaxSeekerSlope:*_ Karakterin çıkabileceği maksimum eğim (yüzde olarak)
