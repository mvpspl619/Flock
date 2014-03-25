var reader = new FileReader();
reader.onload = function (e) {
    var image = new Image();

    image.onload = function () {
        console.log(this.width, this.height);
        if (this.width >= 800 && this.height >= 330) {
            document.getElementById("uplodedPic").style.width = "725px";
            document.getElementById("uplodedPic").style.height = ((this.height * 725) / this.width) + "px";
            document.getElementById("uplodedPic").style.position = "relative";
            console.log("true");
        }
    };
    image.src = e.target.result;
};

var copytexttoimagequack = function() {
    var initialtext = document.getElementById("Textarea1").value;
    document.getElementById("quackText").value = initialtext;
};