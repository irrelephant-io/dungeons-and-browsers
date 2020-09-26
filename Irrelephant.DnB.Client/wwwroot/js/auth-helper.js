async function getCurrentIdToken() {
    const user = await window.AuthenticationService.instance._userManager.getUser();
    return user.id_token;
}