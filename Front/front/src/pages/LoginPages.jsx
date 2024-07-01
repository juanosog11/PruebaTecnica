import { useForm } from "react-hook-form";
import { useAuth } from "../context/AuthContext";
import { useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";

function LoginPages() {
  const { register, handleSubmit, formState: { errors } } = useForm();
  const { ingresar, errores, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    if (isAuthenticated) navigate("/wall");
  }, [isAuthenticated]);

  return (
    <div className="min-h-screen flex items-center justify-center bg-secondary">
      <div className="bg-primary max-w-md p-10 rounded-md shadow-md w-full">
        <h2 className="text-2xl font-bold text-lightblue mb-5 text-center">Ingresar</h2>
        {errores && (
          <div className="bg-red-500 text-white p-2 mb-4 rounded-md">
            {errores}
          </div>
        )}
        <form
          onSubmit={handleSubmit((values) => {
            ingresar(values);
            navigate("/wall")
          })}
        >
          <input
            type="email"
            {...register("Email", { required: true })}
            className="w-full bg-accent text-white px-4 py-2 rounded-md my-2"
            placeholder="Email"
          />
          {errors.Email && (<p className="text-red-500">Ingrese un Email</p>)}
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
            Ingresar
          </button>
        </form>
        <p className="mt-4 text-center text-white">
          ¿No tienes cuenta?
          <Link to="/Registrar" className="text-lightblue ml-1">Registrar</Link>
        </p>
      </div>
    </div>
  );
}

export default LoginPages;
