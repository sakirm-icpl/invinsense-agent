    var keycloak = new Keycloak();
    keycloak.init({onLoad: 'check-sso'}).then(function(authenticated) {
        //console.log(authenticated ? 'authenticated' : 'not authenticated')

        if(authenticated)
        {
            document.getElementById('mainbutton').className = "btn btn-lg btn-success btn-background w-100"; 
        }
        if(!authenticated)
        {
            document.getElementById('mainbutton').className = "btn btn-lg btn-success btn-dark w-100";  
        }

    }).catch(function() {
        console.log('failed to initialize');
        document.getElementById('mainbutton').className = "btn btn-lg btn-success btn-dark w-100";
    });
    
