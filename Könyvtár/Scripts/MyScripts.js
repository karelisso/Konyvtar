//function pageNext(nextpage) {
//    var pagecurrent = parseInt($("#currentpage").text());
//    var pagemax = parseInt($("#maxpage").text());
//    pagecurrent += nextpage;
//    if (pagecurrent > pagemax) pagecurrent = pagemax;
//    else if (pagecurrent < 1) pagecurrent = 1;
//    $("#currentpage").text(pagecurrent);
//    $.ajax({
//        url: "/Default/GetLogBook",
//        type: "GET",
//        data: { 'page': pagecurrent },
//        success: function (data) {
//            var tabledata = "";
//            data.forEach(function (invdata) {
//                console.log(invdata);
//                var milliseconds = parseInt(invdata.logdata.when.replace(/\/Date\((\d+)\)\//, '$1'));
//                var date = new Date(milliseconds);
//                tabledata += "<tr>"
//                tabledata += "<td>" + invdata.logdata.who + "</td>";
//                tabledata += "<td>" + date + "</td>";
//                tabledata += "<td>" + invdata.logtext.text + "</td>";
//                tabledata += "<td>" + invdata.logdata.whom + "</td>";

//                tabledata += "</tr>"
//            });
//            $("tbody").html(tabledata);
//        },
//        error: function () { alert("error"); }
//    });
//}

function autocomplete(inp, arr) {
    console.log("fsdf")
    /*the autocomplete function takes two arguments,
the text field element and an array of possible autocompleted values:*/
    var currentFocus;
    /*execute a function when someone writes in the text field:*/
    inp.addEventListener("input", function (e) {
        var container, items, i, val = this.value;
        /*close any already open lists of autocompleted values*/
        closeAllLists();
        if (!val) { return false; }
        currentFocus = -1;
        /*create a DIV element that will contain the items (values):*/
        container = document.createElement("DIV");
        container.setAttribute("id", this.id + "autocomplete-list");
        container.setAttribute("class", "autocomplete-items");



        container.classList.add("text-dark");
        container.classList.add("dropdown");
        container.classList.add("show");

        // Get the input field
        var inputField = $("#" + inp.id);

        // Get the vertical position of the input field relative to the bottom of the screen
        var windowHeight = $(window).height();
        var inputPosition = inputField.offset().top + inputField.outerHeight();
        var positionPercentage = ((windowHeight - inputPosition) / windowHeight) * 100;
        console.log(positionPercentage);

        //container.style.cssText = "top: " + 60 + "% ; width:auto;";
        /* checkMatchingBook();*/
        //getselectedBook(valu);
        /*append the DIV element as a child of the autocomplete container:*/
        this.parentNode.appendChild(container);
        //document.appendChild(a);

        itemcontainer = document.createElement("ul");
        itemcontainer.setAttribute("class", "dropdown-menu show");
        //itemcontainer.setAttribute("aria-labelledby", inp.id + "-dropdown");
        /*for each item in the array...*/
        for (i = 0; i < arr.length; i++) {
            /*check if the item starts with the same letters as the text field value:*/
            if (arr[i].substr(0, val.length).toUpperCase() == val.toUpperCase()) {
                /*create a DIV element for each matching element:*/


                items = document.createElement("li");
                items.classList.add("dropdown-item");
                /*make the matching letters bold:*/
                items.innerHTML = "<strong>" + arr[i].substr(0, val.length) + "</strong>";
                items.innerHTML += arr[i].substr(val.length);
                /*insert a input field that will hold the current array item's value:*/
                items.innerHTML += "<input type='hidden' value='" + arr[i] + "'>";
                /*execute a function when someone clicks on the item value (DIV element):*/
                items.addEventListener("click", function (e) {
                    /*insert the value for the autocomplete text field:*/
                    inp.value = this.getElementsByTagName("input")[0].value.split("/")[0];
                    $("#konyvname").text(() => {
                        return this.getElementsByTagName("input")[0].value.split("/")[1];
                    })
                    szidstart();
                    /* checkMatchingBook();*/
                    /*close the list of autocompleted values,
                    (or any other open lists of autocompleted values:*/
                    closeAllLists();
                });
                container.appendChild(itemcontainer);
                itemcontainer.appendChild(items);


            }
        }
    });




    /*execute a function presses a key on the keyboard:*/
    inp.addEventListener("keydown", function (e) {
        var x = document.getElementById(this.id + "autocomplete-list");
        if (x) x = x.getElementsByTagName("li");
        if (e.keyCode == 40) {
            /*If the arrow DOWN key is pressed,
            increase the currentFocus variable:*/
            currentFocus++;
            /*and and make the current item more visible:*/
            addActive(x);
        } else if (e.keyCode == 38) { //up
            /*If the arrow UP key is pressed,
            decrease the currentFocus variable:*/
            currentFocus--;
            /*and and make the current item more visible:*/
            addActive(x);
        }

        else if (e.keyCode == 13) {
            /*If the ENTER key is pressed, prevent the form from being submitted,*/
            e.preventDefault();
            if (currentFocus > -1) {
                /*and simulate a click on the "active" item:*/
                if (x) {

                    x[currentFocus].click();
                    // setTimeout("checkMatchingBook", 20);
                }
            }
        }
    });
    function addActive(x) {
        /*a function to classify an item as "active":*/
        if (!x) return false;
        /*start by removing the "active" class on all items:*/
        removeActive(x);
        if (currentFocus >= x.length) currentFocus = 0;
        if (currentFocus < 0) currentFocus = (x.length - 1);
        /*add class "autocomplete-active":*/
        x[currentFocus].classList.add("autocomplete-active");
    }
    function removeActive(x) {
        /*a function to remove the "active" class from all autocomplete items:*/
        for (var i = 0; i < x.length; i++) {
            x[i].classList.remove("autocomplete-active");
        }
    }
    function closeAllLists(elmnt) {
        /*close all autocomplete lists in the document,
        except the one passed as an argument:*/
        var x = document.getElementsByClassName("autocomplete-items");
        for (var i = 0; i < x.length; i++) {
            if (elmnt != x[i] && elmnt != inp) {
                x[i].parentNode.removeChild(x[i]);
            }
        }
    }
    /*execute a function when someone clicks in the document:*/
    document.addEventListener("click", function (e) {
        closeAllLists(e.target);
    });
}