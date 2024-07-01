import { useForm } from "react-hook-form";
import { useAuth } from "../context/AuthContext";
import { useEffect } from "react";
import { useNavigate, Link } from "react-router-dom";

function RegistrarPages() {
    const { register, handleSubmit, formState:{errors} } = useForm();
    const { registrar, isAuthnticated, errores } = useAuth()
    const navigate = useNavigate()

    useEffect(()=> {
        if (isAuthnticated) navigate("/wall");
        
    },[isAuthnticated]);

    return (
        <div className="min-h-screen flex items-center justify-center bg-secondary">
            <div className="bg-primary max-w-md p-10 rounded-md shadow-md w-full">
                <h2 className="text-2xl font-bold text-lightblue mb-5 text-center">Registrar</h2>
                {errores && (
                    <div className="bg-red-500 text-white p-2 mb-4 rounded-md">
                        {errores}
                    </div>
                )}
                <form
                    onSubmit={handleSubmit(async (values) => {
                        registrar(values)
                        navigate("/wall")
                    })}
                >
                    {/* control de input, lo que esta entre {} es de useForm que viene del register y tiene su login tambien */}
                    <input
                        type="text"
                        {...register("Usuario", { required: true })}
                        className="w-full bg-accent text-white px-4 py-2 rounded-md my-2"
                        placeholder="Username"
                    />
                    {/* Manejo de errores */}
                    {errors.Usuario && (<p className="text-red-500">Ingrese un usuario</p>)}
                    <input
                        type="email"
                        {...register("Email", { required: true })}
                        className="w-full bg-accent text-white px-4 py-2 rounded-md my-2"
                        placeholder="Email"
                    />
                    {errors.Email && (<p className="text-red-500">Ingrese un Email</p>)}
                    <input
                        type="text"
                        {...register("Numero", { required: true })}
                        className="w-full bg-accent text-white px-4 py-2 rounded-md my-2"
                        placeholder="Numero"
                    />
                    {errors.Numero && (<p className="text-red-500">Ingrese un numero de celular</p>)}
                    <input
                        type="password"
                        {...register("Clave", { required: true })}
                        className="w-full bg-accent text-white px-4 py-2 rounded-md my-2"
                        placeholder="Password"
                    />
                    {errors.Clave && (<p className="text-red-500">Ingrese una Clave</p>)}
                    <button
                        type="submit"
                        className="w-full bg-blue text-white px-4 py-2 rounded-md mt-4 hover:bg-lightblue transition-colors"
                    >
                        Register
                    </button>
                    <p className="mt-4 text-center text-white">
                        ¿No tienes cuenta?  
                        <Link to="/Ingresar" className="text-lightblue ml-1">  Ingresar</Link>
                    </p>
                </form>
            </div>
        </div>
    );
}

export default RegistrarPages;
