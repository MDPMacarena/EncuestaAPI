const menu = document.querySelector("#menu");
const menuMovil = document.querySelector("#menuMovil");
    menu.addEventListener("click", function(){
        if(menuMovil.style.translate == "-70vw"){
             menuMovil.style.translate ="0vw";
        }else{
            menuMovil.style.translate ="-70vw";
        }
        
    })

let id, nombre, token;
function parseJwt(token) {
    const payload = token.split('.')[1];
    return JSON.parse(atob(payload));
}
//function cargarUsuario() {
//    token = sessionStorage.getItem("token");
//    if (!token) {
//        window.location.href = "inicio.html";
//        return;
//    }
//    const claims = parseJwt(token);
//    id = claims.id;
//    nombre = claims["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"];
//    document.querySelector("#cerrarSesion").addEventListener("click", cerrarSesion);
//}
function cerrarSesion() {
    sessionStorage.removeItem("token");
    window.location.href = "index.html";
}
/*cargarUsuario();*/