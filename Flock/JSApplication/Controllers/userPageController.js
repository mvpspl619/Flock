
'use strict';

flockApp.controller('userPageController', function ($scope, $window, userService, quackService) {
    $scope.quackCount = null;
    $scope.loadedQuacks = 0;
    $scope.displayName = "";
    $scope.showConversations = false;
    $scope.expandOrCollapse = "Expand";
    $scope.maxCharacters = 300;
    $scope.user = {};
    $scope.quacks = [];
    $scope.userPreferences = "User Preferences";
    $scope.userProfilePicUrl = "";
    $scope.replyMode = false;
    $scope.userLikeQuackId = "";
    $scope.disableQuackMessage = false;
    $scope.disableReplyAction = false;
    $scope.isBusy = false;
    $scope.deletedQuacks = [];
    var isValidUser = false;

    $scope.setQuackId = function(quackId, likes) {
            $scope.userLikeQuackId = quackId;
            $scope.$broadcast('quackUserLikesController.showUserLikes');
    };

    $scope.showUserDetails = function() {
        $scope.$broadcast('userInfoController.showUserInformation');
    };

    //var getQuacks = function () {
    //    $scope.refreshQuacks($scope.loadedQuacks);
    //    $scope.loadedQuacks = $scope.loadedQuacks + 10;
    //};

    userService.getUser().then(function (user) {
        
        if (user == "null") {
           
            window.location.replace("/JSApplication/Templates/Guest.html");
            return;
        }
        isValidUser = true;
        $scope.user = user;
        $scope.displayName = user.FirstName + " " + user.LastName;
        
        if (user.CoverImage == "") {
            $("#userCoverPic").attr("src", "/Content/images/defaultCoverPic.png");
            $scope.imageUrl = "/Content/images/defaultCoverPic.png";
        }
        else {
            $("#userCoverPic").attr("src", "data:image/jpeg;base64," + user.CoverImage);
            $scope.imageUrl = "data:image/jpeg;base64," + user.CoverImage;
        }

        if(user.ProfileImage =="") {
            $scope.userProfilePicUrl = "/Content/images/profilepic.png";
            $scope.profilePicimageUrl = $scope.userProfilePicUrl;
        }
        else {
            $scope.userProfilePicUrl = "data:image/jpeg;base64," + user.ProfileImage;
            $scope.profilePicimageUrl = $scope.userProfilePicUrl;
        }
        $scope.getNewQuacks();
    });

    $scope.saveQuack = function () {
        
        if(!isValidUser ) {
            return;
        }
        
        $scope.disableQuackMessage = true;
        var quack = {};
        quack.userId = $scope.user.ID;
        quack.parentQuackId = null;
        quack.quackTypeId = 1;
        quack.quackContent = {};
        quack.quackContent.messageText = $scope.messageContent;
        

        if (quack.quackContent.messageText != "") {
            quackService.saveQuack(quack).then(function (data) {
                $scope.reloadQuackNew(data);
                $scope.replyMode = false;
                $scope.messageContent = "";
                $scope.disableQuackMessage = false;
                
            });
        }
        else {
            $scope.disableQuackMessage = false;
        }
    };

    $scope.saveReply = function (passedQuack, element, isNew, conversationId) {
        $scope.disableReplyAction = true;
        var quack = {};
        var finalQuack;
        quack.userId = $scope.user.ID;
        quack.parentQuackId = null;
        quack.quackTypeId = 2;
        quack.quackContent = {};
        
        if (isNew) {
            quack.conversationId = passedQuack.Id;
        }
        else {
            quack.conversationId = conversationId;
        }
        quack.quackContent.messageText = $('#'+element).val();
        if (quack.quackContent.messageText != "") {
            quackService.saveQuack(quack).then(function (data) {
                if (data != null) {
                    //reload the quack
                    $scope.reloadQuack(passedQuack.Id, $scope.quacks.indexOf(passedQuack), "Comment");
                }
                $scope.replyMode = false;
                $scope.replyContent = "";
                $scope.disableReplyAction = false;
            });
        }
        else {
            $scope.disableReplyAction = false;
        }
    };

    $scope.getNewQuacks = function() {
        if ($scope.isBusy != true)
            $scope.getQuacks($scope.loadedQuacks);
    };

    $scope.getQuacks = function (count) {
        $scope.isBusy = true;
        if ($scope.quackCount === $scope.loadedQuacks)
            return;
        quackService.getAllQuacks(count).then(function(data) {
            for (var i = 0; i < data.Quacks.length; i++) {

                //look for hashtag and provide a href on click
                var postText = data.Quacks[i].Message;
                //var regexp = new RegExp('#([^\\s]*)', 'g');
                var hashtag = postText.match(/#\w+/g);
                if (hashtag !== null) {
                    for (var k = 0; k < hashtag.length; k++) {
                        var hashtagLink = "<a href='/SearchView/index?hashtag=" + hashtag[k].replace("#", "") + "'>" + hashtag[k] + "</a>";
                        //var hashtagLink = "<a href=''" + " ng-click=" + "openHashtagPage(" + "'" + hashtag[k] + "'" + ')' + '>' + hashtag[k] + '</a>';
                        postText = postText.replace(hashtag[k], hashtagLink);
                    }
                    data.Quacks[i].Message = postText;
                }

                if (!(data.Quacks[i].QuackImage) || data.Quacks[i].QuackImage == "") {
                    data.Quacks[i].showQuackImage = false;
                }
                else {
                    //data[i].QuackImage = "data:image/jpeg;base64," + data[i].QuackImage;
                    data.Quacks[i].showQuackImage = true;
                }

                if (!(data.Quacks[i].UserImage) || data.Quacks[i].UserImage == "") {
                    data.Quacks[i].UserImage = "/Content/images/profilepic.png";
                }
                //else {
                //    data[i].UserImage = "data:image/jpeg;base64," + data[i].UserImage;
                //}

                if (data.Quacks[i].Replies != 0) {
                    data.Quacks[i].showLatestReply = true;
                    if (!(data.Quacks[i].LatestReply.UserImage) || data.Quacks[i].LatestReply.UserImage == "") {
                        data.Quacks[i].LatestReply.UserImage = "/Content/images/profilepic.png";
                    }
                    //else {
                    //    data[i].LatestReply.UserImage = "data:image/jpeg;base64," + data[i].LatestReply.UserImage;
                    //}
                    if ($scope.user.ID == data.Quacks[i].LatestReply.UserId) {
                        data.Quacks[i].LatestReply.ShowDelete = true;
                    } else {
                        data.Quacks[i].LatestReply.ShowDelete = false;
                    }
                }
                else {
                    data.Quacks[i].showLatestReply = false;
                }
                data.Quacks[i].ShowConversation = false;
                data.Quacks[i].ExpandOrCollapse = "Expand";
                if ($scope.user.ID == data.Quacks[i].UserId) {
                    data.Quacks[i].ShowDelete = true;
                } else {
                    data.Quacks[i].ShowDelete = false;
                }
                for (var j = 0; j < data.Quacks[i].QuackReplies.length ; j++) {

                    if (!(data.Quacks[i].QuackReplies[j].UserImage) || data.Quacks[i].QuackReplies[j].UserImage == "") {
                        data.Quacks[i].QuackReplies[j].UserImage = "/Content/images/profilepic.png";
                    }
                    //else {
                    //    data[i].QuackReplies[j].UserImage = "data:image/jpeg;base64," + data[i].QuackReplies[j].UserImage;
                    //}
                }
                $scope.quacks.push(data.Quacks[i]);
            }
            $scope.loadedQuacks = $scope.loadedQuacks + data.Quacks.length;
            $scope.quackCount = data.QuackCount;
            $scope.isBusy = false;
        });
    };

    $scope.expandClick = function (quack) {
        
        quack.ShowConversation = !quack.ShowConversation;
        quack.ExpandOrCollapse = quack.ExpandOrCollapse == "Expand" ? "Collapse" : "Expand";
        
        if(quack.ExpandOrCollapse == "Expand") {
            $scope.replyMode = false;
        }
        else {
            $scope.replyMode = true;
        }

        if ($scope.user.ID == quack.LatestReply.UserId) {
            quack.LatestReply.ShowDelete = true;
        } else {
            quack.LatestReply.ShowDelete = false;
        }

       for(var i=0; i<$scope.quacks.length  ; i++) {
            if(quack.Id == $scope.quacks[i].Id ) {
                
            }
            else {
                $scope.quacks[i].ExpandOrCollapse = "Expand";
                $scope.quacks[i].ShowConversation = false;
            }
           
        }
       if (quack.ExpandOrCollapse == "Collapse") {
           quackService.getQuackInformation(quack.Id).then(function(data)
           {
               for (var f = 0; f < data.length ; f++) {
                   if (!(data[f].UserImage) || data[f].UserImage == "") {
                       data[f].UserImage = "/Content/images/profilepic.png";
                   }
                   //else {
                   //    data[f].UserImage = "data:image/jpeg;base64," + data[f].UserImage;
                   //}
                   
                   if ($scope.user.ID == data[f].UserId) {
                       data[f].ShowDelete = true;
                   } else {
                       data[f].ShowDelete = false;
                   }
                   

               }
               quack.QuackReplies = data;
             
           });
       }
    };

    $scope.deleteQuack = function (quack) {
        //put the quackId in deletedQuacks list
        $scope.deletedQuacks.push(quack);
        quackService.deleteQuack(quack.Id).then(function () {
            var qId = "#quack" + quack.Id;
            var undoId = "#quackUndo" + quack.Id;
            $(qId).hide();
            $(undoId).show();
        });
    };

    $scope.undoDelete = function(quack) {
        $scope.deletedQuacks.pop(quack);
        quackService.activateQuack(quack.Id).then(function () {
            var qId = "#quack" + quack.Id;
            var undoId = "#quackUndo" + quack.Id;
            $(qId).show();
            $(undoId).hide();
        });
    };

    $scope.likeOrUnlikeQuack = function (quack) {
        quackService.likeOrUnlikeQuack(quack.Id, $scope.user.ID,
            quack.LikeOrUnlike == "like" ? true : false).then(function (data) {
            if (data == "true") {
                //reload the quack
                $scope.reloadQuack(quack.Id, $scope.quacks.indexOf(quack));
            }
        } );
    };

    $scope.reloadQuack = function(quackId, quackIndex, type) {
        quackService.reloadQuack(quackId).then(function (data) {
            if (!(data.QuackImage) || data.QuackImage == "") {
                data.showQuackImage = false;
            }
            else {
                data.showQuackImage = true;
            }
            if ($scope.user.ID == data.LatestReply.UserId) {
                data.LatestReply.ShowDelete = true;
            } else {
                data.LatestReply.ShowDelete = false;
            }
            if ($scope.user.ID == data.UserId) {
                data.ShowDelete = true;
            } else {
                data.ShowDelete = false;
            }
            $scope.quacks[quackIndex] = $scope.updateHashtag(data);
            //AWESOME JOB BRO
        });
    };

    $scope.reloadQuackNew = function (quackId) {
        quackService.reloadQuack(quackId).then(function (data) {
            if (!(data.QuackImage) || data.QuackImage == "") {
                data.showQuackImage = false;
            }
            else {
                data.showQuackImage = true;
            }
            if ($scope.user.ID == data.LatestReply.UserId) {
                data.LatestReply.ShowDelete = true;
            } else {
                data.LatestReply.ShowDelete = false;
            }
            if ($scope.user.ID == data.UserId) {
                data.ShowDelete = true;
            } else {
                data.ShowDelete = false;
            }
            data = $scope.updateHashtag(data);
            $scope.quacks.unshift(data);
            //handle assignment of data:base64 to img

            //AWESOME JOB BRO
        });
    };

    $scope.updateHashtag = function(quack) {
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

    $scope.$on('userTagSelected', function (event, data) {       
        data.firstName = (data.firstName || "").split("@")[1]; // purpose: remove @ from first name
        //sessionFactory.user = data;
        openProfilePage(data.firstName, data.lastName);
    });

    $scope.openProfilePageByGivenName = function (data) {
        var names = data.split(" ");
        openProfilePage(names[0], names[1]);
    };

    var openProfilePage = function(firstName, lastName) {
        $window.open('/UserView/index?firstName=' + firstName + "&lastName=" + lastName, lastName + firstName);
    };

    $scope.openSearchPagebyHashtag = function (data) {
        data = data.replace("#", "");
        openHashtagPage(data);
    };

    var openHashtagPage = function (hashtag) {
        hashtag = hashtag.replace("#", "");
        $window.open('/Search/search?hashtag=' + hashtag);
    };
});

