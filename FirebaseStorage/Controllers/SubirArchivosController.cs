using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FirebaseStorage.Controllers
{
    public class SubirArchivosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> SubirArchivo(IFormFile archivo)
        {
            //Leemos el archivo subido
            Stream archivoASubir = archivo.OpenReadStream();

            //Configuramos la conexion hacia el FireBase
            string email = "alanis.alvarez@catolica.edu.sv";
            string clave = "Thewoomy1029";
            string ruta = "gs://practicafirebasestorage.appspot.com"; //Se ve en storage
            string api_key = "AIzaSyCx-Clb-LL1phdldyh8E-WT2tD4JQpdNCc"; //Se ve en configuración

            //Esto es para poder autenticarse con el correo y contraseña
            var auth = new FirebaseAuthProvider(new FirebaseConfig(api_key));
            var authenticarFireBase = await auth.SignInWithEmailAndPasswordAsync(email, clave);

            //token de cancelacion y de usuario requeridpos para enviar el archivo
            var cancellation = new CancellationTokenSource();
            var tokenUser = authenticarFireBase.FirebaseToken;

            //Configuramos la ruta de envío al storage (ni idea de porque me da error)
            var tareaCargarArchivo = new FirebaseStorage(ruta,
                                                               new FirebaseStorageOptions
                                                               {
                                                                   AuthTokenAsyncFactory = () => Task.FromResult(tokenUser),
                                                                   ThrowOnCancel = true

                                                               }
                                                         ).Child("Archivo").Child(archivo.FileName).PutAsync(archivoASubir, cancellation.Token);

            var urlArchivoCargado = await tareaCargarArchivo;


            return RedirectToAction("VerImagen");

        }

    }




}
