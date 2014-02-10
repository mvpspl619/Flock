'use strict';

flockApp.controller('uploadImageController', function ($scope, userService, quackService, $timeout) {
    $scope.showErrorMessage = false;
    $scope.showPreview = true;
    $scope.showSave = false;
    $scope.imageSource = "";
    $scope.coverPicImageSource = "";

    $timeout(function () {
        $scope.coverPicImageSource = $scope.imageUrl;
    }, 2000);

    $scope.imageSourceOriginal = "";


    var readUrl = function (input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {

                var userImage = {};
                userImage.sourceUrl = e.target.result;
                userImage.action = "VerifyCoverPic";
                userService.uploadImage(userImage).then(function (data) {

                    if (data.Message == "true") {
                        $scope.showErrorMessage = false;
                        $scope.imageSource = e.target.result;
                        $scope.imageSourceOriginal = e.target.result;
                        $('#uplodedPic').attr('src', e.target.result);
                        $scope.showPreview = true;
                        $scope.showSave = false;
                    } else {
                        $scope.showErrorMessage = true;

                        $timeout(function () {
                            $scope.showErrorMessage = false;
                        }, 4000);
                    }
                });
            };
            reader.readAsDataURL(input.files[0]);
        }
    };

    $('#uploadedPic').focusout(function () {
        $('#uplodedPic').imgAreaSelect({
            hide: true
        });
    });


    $("#imgProfilePic").change(function () {
        readUrl(this);
    });

    $scope.imagePreview = function () {
        var userImage = {};
        userImage.sourceUrl = $scope.imageSource;
        userImage.x = 0;
        var y = $("#output").val();
        y = y.replace(/[^\d]/g, '');
        userImage.y = y;
        userImage.width = 880;
        console.log($("uplodedPic"));
        var element = document.getElementById('uplodedPic');
        var imgHeight = angular.element(element).css('height');
        imgHeight = imgHeight.replace(/[^\d]/g, '');
        userImage.height = imgHeight;

        //if (!(userImage.x == "" || userImage.y == "")) {
            userImage.action = "PreviewCoverPic";
            userService.uploadImage(userImage).then(function (data) {
                $("#uplodedPic").attr("src", "data:image/jpeg;base64," + data.Message);
                $scope.imageSource = "data:image/jpeg;base64," + data.Message;
                $("#uplodedPic").attr("style", "style='width:880px; height:330px;'");
                $scope.showPreview = false;
                $scope.showSave = true;
            });
        //}
    };

    $scope.saveImage = function () {
        var userImage = {};
        userImage.sourceUrl = $scope.imageSource;
        userImage.action = "SaveCoverPic";
        userImage.userId = $scope.user.ID;
        $scope.imageUrl = $scope.imageSource;
        $("#userCoverPic").attr("src", $scope.imageUrl);
        userService.uploadImage(userImage).then(function (data) {
            $scope.showPreview = false;
            $scope.showSave = true;
            $scope.imageUrl = data.Message;
            $("#X").val(0);
            $("#Y").val(0);

        });
    };

    $scope.reset = function () {
       $("#uplodedPic").attr("src", $scope.imageSourceOriginal);
        $scope.imageSource = angular.copy($scope.imageSourceOriginal);
        $scope.showPreview = true;
        $scope.showSave = false;
        $("#X").val(0);
        $("#Y").val(0);
    };
});