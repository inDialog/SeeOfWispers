// $ npm install --save line-reader

var id;
var state = false;
var adress = './Log/' + "keyList" + '.json'
const fs = require('fs');
const lineReader = require('line-reader');
//////TODO check file for recurent key
var createKey = function() {
	while (!state) {
		a =Math.floor(Math.random()*50 +1);
		b =Math.floor(Math.random()*500 +1);
		c =Math.floor(Math.random()*1000 +1);
		d =Math.floor(Math.random()*50 +1);
		var idKey =  a+'-'+b+'-'+c+'-'+d;
  	state = CheckKey(idKey);
    }
    // while(){

    // }
  	state = false;
  	SaveKey("\n"+idKey.toString()+'\t'+TimwNow().toString());
	return idKey;
};
function  LookForKey (data,callback){
		lineReader.eachLine(adress, function(line,last) {
		 	if(line.includes(data))
		 	{
    		console.log(line);
   			return callback(true);
  			}
		});

}

var CheckKey = function(key){
    var a, b, c, d;
    var _key;
    _key = key.split('-');
    a = parseInt(_key[0]);
    b = parseInt(_key[1]);
    c = parseInt(_key[2]);
    d = parseInt(_key[3]);
	var floor = ((a*b)+(c/d))+a*d;
    return floor == 1992;
}
var SaveKey = function(key) {

	fs.appendFile(adress, key,'utf8', err => {
    	if (err) {
       		console.log('Error writing file', err)
  		}
	})
};
var TimwNow = function() {
	var currentdate = new Date(); 
	var datetime = "Requested on: " + currentdate.getDate() + "/"
                + (currentdate.getMonth()+1)  + "/" 
                + currentdate.getFullYear() + " @ "  
                + currentdate.getHours() + ":"  
                + currentdate.getMinutes() + ":" 
                + currentdate.getSeconds();
        	// console.log(datetime)

	return datetime;

};

module.exports.LookForKey = LookForKey;

module.exports.createKey = createKey;