import { createContext, useState, useContext, useEffect } from "react";
import { registerUser as apiRegister, Ingresar as apiIngresar, verificarToken, CerrarSesion as apiCerrarSesion } from "../api/auth";

export const AuthContext = createContext();

export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error("useAuth must be used within an AuthProvider");
    }
    return context;
};

export const AuthProvider = ({ children }) => {
    const [usuario, setUsuario] = useState(null);
    const [selectedUsername, setSelectedUsername] = useState(""); // Nuevo estado para almacenar el nombre de usuario seleccionado
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [errores, setErrores] = useState(null);
    const [loading, setLoading] = useState(true);

    const registrar = async (user) => {
        try {
            const res = await apiRegister(user);
            setUsuario(res.Usuario);
            setIsAuthenticated(true);
        } catch (error) {
            setErrores(error.response.data.message);
            console.log(error.response.data.message);
        }
    };

    const ingresar = async (user) => {
        try {
            const res = await apiIngresar(user);
            setUsuario(res.Usuario);
            setIsAuthenticated(true);
        } catch (error) {
            setErrores(error.response.data.message);
            console.log(error);
        }
    };

    const logout = async () => {
        try {
            const res = await apiCerrarSesion();
            if (res.isSuccess) {
                setUsuario(null);
                setIsAuthenticated(false);
            } else {
                console.error(res.message);
            }
        } catch (error) {
            console.error("Error logging out:", error);
        }
    };

    useEffect(() => {
        const checkToken = async () => {
            try {
                const response = await verificarToken();
                if (response.isSuccess) {
                    setLoading(false);
                    setIsAuthenticated(true);
                    setUsuario(response.usuario);
                } else {
                    console.log(response.message); // Mostrar mensaje si no se encuentra el token
                    setIsAuthenticated(false);
                    setUsuario(null);
                    setLoading(false);
                }
            } catch (error) {
                console.error('Error al verificar token:', error);
                setLoading(false);
            }
        };

        checkToken();
    }, []);

    useEffect(() => {
        if (errores != null) {
            const timer = setTimeout(() => {
                setErrores(null);
            }, 5000);
            return () => clearTimeout(timer);
        }
    }, [errores]);

    return (
        <AuthContext.Provider value={{ registrar, usuario, isAuthenticated, errores, ingresar, logout, loading, selectedUsername, setSelectedUsername }}>
            {children}
        </AuthContext.Provider>
    );
};
