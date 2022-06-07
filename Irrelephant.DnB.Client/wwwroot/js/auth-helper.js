function getCurrentIdToken(oidcTokenKey) {
    let token = window.sessionStorage.getItem(oidcTokenKey);
    if (token !== undefined) {
        return JSON.parse(token).id_token;
    }
    
    return null;
}