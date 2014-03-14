'use strict';

flockApp.controller('postImageQuackController', function ($scope, userService, quackService, $timeout) {
    
    $scope.quackPicImageSource = "";
    $scope.showQuackPictureModalErrorMessage = false;
    //$timeout(function () {
    //    $scope.profilePicImageSource = $scope.profilePicimageUrl;
    //}, 2000);


    var readUrl = function (input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                //var userImage = {};
                //userImage.sourceUrl = e.target.result;
                //userImage.action = "postImageQuack";
                //userService.uploadImage(userImage).then(function (data) {

                    //if (data.Message == "true") {
                        $scope.showPictureModalErrorMessage = false;
                        $('#uplodedQuackPicture').attr('src', e.target.result);
                        $scope.quackPicImageSource = e.target.result;
                       
                    //}
                    //else {
                    //    $scope.showPictureModalErrorMessage = true;
                    //    $timeout(function () {
                    //        $scope.showPictureModalErrorMessage = false;
                    //    }, 4000);
                    //}
                //});
            };
            reader.readAsDataURL(input.files[0]);
        }
    };

    $("#UploadQuackPic").change(function () {
     
        readUrl(this);
    });


    $scope.saveImageQuack = function () {
        
        $scope.disableQuackImageMessage = true;
        var quack = {};
        quack.userId = $scope.user.ID;
        quack.parentQuackId = null;
        quack.quackTypeId = 1;
        quack.quackContent = {};
        quack.quackContent.messageText = $scope.imageQuackMessageContent;
        quack.quackContent.imageUrl = $scope.quackPicImageSource;
        
        if ($scope.imageQuackMessageContent && $scope.imageQuackMessageContent!= "") {
            quackService.saveQuack(quack).then(function (data) {
                $scope.reloadQuackNew(data);
                $scope.replyMode = false;
                $scope.imageQuackMessageContent = "";
                $scope.disableQuackImageMessage = false;
                $('#postImageQuack').modal('hide');
            });
    
        }
        else {
            $scope.disableQuackImageMessage = false;
            $scope.showQuackPictureModalErrorMessage = true;
            $timeout(function () {
                $scope.showQuackPictureModalErrorMessage = false;
            }, 4000);
            return false;
        }
        
    };

    $scope.reloadQuackNew = function (quackId) {
        quackService.reloadQuack(quackId).then(function (data) {
            if (!(data.QuackImage) || data.QuackImage == "") {
                data.showQuackImage = false;
            }
            else {
                data.showQuackImage = true;
            }
            data = $scope.updateHashtag(data);
            $scope.quacks.unshift(data);
            //handle assignment of data:base64 to img

            //AWESOME JOB BRO
        });
    };

    $scope.updateHashtag = function (quack) {
        var postText = quack.Message;
        //var regexp = new RegExp('#([^\\s]*)', 'g');
        var hashtag = postText.match(/#\w+/g);
        if (hashtag !== null) {
            for (var k = 0; k < hashtag.length; k++) {
                var hashtagLink = "<a href='/SearchView/index?hashtag=" + hashtag[k].replace("#", "") + "'>" + hashtag[k] + "</a>";
                //var hashtagLink = "<a href=''" + " ng-click=" + "openHashtagPage(" + "'" + hashtag[k] + "'" + ')' + '>' + hashtag[k] + '</a>';
                postText = postText.replace(hashtag[k], hashtagLink);
            }
            quack.Message = postText;
        }
        return quack;
    };

});