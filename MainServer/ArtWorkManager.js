const fs = require('fs');
const path = require('path');
const app = require('./app.js')
const keysGen = require('./keyGen.js')

module.exports.ComposeString = ComposeString;
module.exports.ArtWork = ArtWork;


const directoryPath = path.join(__dirname, 'Save');

function ArtWork (data,callback){
    var [udid,
        X, Y, Z,
        sX, sY, sZ,
        pX, pY, pZ,
        rX,rY,rZ,
        cX,cY,cZ,
        inUrl,inDescription, options,
        verStat] =  data.toString().split('\t');
        
        var artWroks ={};

      artWroks[udid] = {
            position: {
                x: parseFloat(X),
                y: parseFloat(Y),
                z: parseFloat(Z)
            },  
            artworkScale: {
                x: parseFloat(sX),
                y: parseFloat(sY),
                z: parseFloat(sZ)
            },
            platform: {
                x: parseFloat(pX),
                y: parseFloat(pY),
                z: parseFloat(pZ)
            },
            rotation: {
                x: parseFloat(rX),
                y: parseFloat(rY),
                z: parseFloat(rZ)
            },
            colideScale: {
                x: parseFloat(cX),
                y: parseFloat(cY),
                z: parseFloat(cZ)
            },
            url : inUrl,
            description : inDescription,
            uploadOptions : options,
            verifiedStatus :verStat,
            id : udid
      }
    keysGen.LookForKey(udid, function(response){
    const jsonString = JSON.stringify(artWroks, null, 2)
    var adress = './Save/' + udid + '.json'
    fs.writeFile(adress, jsonString, err => {
        if (err) {
            console.log('Error writing file', err)
        } else {
            console.log('Successfully wrote file')
            return callback(true)
        }   
    })
        // app.SendArtworks(null,false);
        // console.log(response);
            })
}

function ComposeString  (callback) {
 var artWroks = {}

    fs.readdir(directoryPath, function (err, files,) {
    if (err) {
        return console.log('Unable to scan directory: ' + err);
    } 
    var  i =0;
    files.forEach(function (file,index,array) {
    	console.log(array)
      var len = array.length;
    	var [id,type] =file.toString().split('.');
    	if('json'===type){
        
    		    jsonReader('./Save/' +  file, (err, recived) => {
                         i+=1;
   					 if (err) {
       					 console.log(err)
        				return
    					}
    				console.log("arraySize " + len)
                    console.log("index  " + i)
             
    				artWroks[id] = recived[id]
    				// temp[id] += JSON.stringify(recived[id],null);
    				// var tm = JSON.stringify(recived);
    				// temp[id] += tm
    				 if (i == len){ 
    				return callback(artWroks)
   						}

				})
        // console.log(file.toString()); 
    }

    });
});


}
 
function jsonReader(filePath, cb) {
    fs.readFile(filePath, (err, fileData) => {
        if (err) {
            return cb && cb(err)
        }
        try {
            const object =  JSON.parse(fileData)
            return cb && cb(null,object)
        } catch(err) {
            return cb && cb(err)
        }
    })
}