import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { updateUser } from '../api/auth';
import CryptoJS from 'crypto-js'; // Importar CryptoJS para la encriptación AES

function EditarPerfilPages() {
    const navigate = useNavigate();
    const { usuario } = useAuth();

    const [formValues, setFormValues] = useState({
        usuarioId: usuario.usuarioId,
        usuario: usuario.usuario,
        email: usuario.email,
        numero: usuario.numero,
        clave: '', // No mostrar la clave en un campo de texto visible
    });
    const [isLoading, setIsLoading] = useState(false);

    useEffect(() => {
        setFormValues({
            usuarioId: usuario.usuarioId,
            usuario: usuario.usuario,
            email: usuario.email,
            numero: usuario.numero,
            clave: '', // No mostrar la clave en un campo de texto visible
        });
    }, [usuario]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormValues((prevValues) => ({
            ...prevValues,
            [name]: value,
        }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            setIsLoading(true);

            // Solo encriptar la clave si ha sido modificada
            let updatedValues = { ...formValues };
            if (formValues.clave) {
                const encryptedClave = encryptPassword(formValues.clave);
                updatedValues = { ...updatedValues, clave: encryptedClave };
            }

            await updateUser(formValues.usuarioId, updatedValues);
            setIsLoading(false);
            navigate('/perfil');
        } catch (error) {
            console.error('Error updating user profile:', error);
            setIsLoading(false);
            // Manejar errores, por ejemplo, mostrar un mensaje al usuario
        }
    };

    // Función para encriptar la contraseña utilizando AES
    const encryptPassword = (password) => {
        const key = CryptoJS.enc.Utf8.parse('1234567890123456'); // Ejemplo de clave (debería ser segura y privada)
        const iv = CryptoJS.enc.Utf8.parse('1234567890123456'); // Ejemplo de IV (debería ser seguro y privado)

        const encrypted = CryptoJS.AES.encrypt(
            password,
            key,
            {
                iv: iv,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7,
            }
        );

        return encrypted.toString();
    };

    return (
        <div className="bg-primary min-h-screen text-lightblue p-4">
            <h1 className="text-4xl font-bold mb-6 text-center text-blue">Editar Perfil de {formValues.usuario}</h1>
            <form onSubmit={handleSubmit} className="max-w-md mx-auto">
                <div className="mb-4">
                    <label htmlFor="usuario" className="block mb-2">
                        Nombre de usuario:
                    </label>
                    <input
                        type="text"
                        id="usuario"
                        name="usuario"
                        value={formValues.usuario}
                        onChange={handleChange}
                        className="w-full p-2 border rounded"
                        required
                    />
                </div>
                <div className="mb-4">
                    <label htmlFor="email" className="block mb-2">
                        Email:
                    </label>
                    <input
                        type="email"
                        id="email"
                        name="email"
                        value={formValues.email}
                        onChange={handleChange}
                        className="w-full p-2 border rounded"
                        required
                    />
                </div>
                <div className="mb-4">
                    <label htmlFor="numero" className="block mb-2">
                        Número de teléfono:
                    </label>
                    <input
                        type="text"
                        id="numero"
                        name="numero"
                        value={formValues.numero}
                        onChange={handleChange}
                        className="w-full p-2 border rounded"
                    />
                </div>
                <div className="mb-4">
                    <label htmlFor="clave" className="block mb-2">
                        Clave:
                    </label>
                    <input
                        type="password"
                        id="clave"
                        name="clave"
                        value={formValues.clave}
                        onChange={handleChange}
                        className="w-full p-2 border rounded"
                        required
                    />
                </div>
                <button
                    type="submit"
                    className="bg-blue text-primary py-2 px-4 rounded hover:bg-lightblue transition duration-300 mr-2"
                    disabled={isLoading}
                >
                    {isLoading ? 'Guardando...' : 'Guardar'}
                </button>
                <button
                    type="button"
                    className="bg-red-600 text-primary py-2 px-4 rounded hover:bg-red-700 transition duration-300"
                    onClick={() => navigate('/perfil')}
                    disabled={isLoading}
                >
                    Cancelar
                </button>
            </form>
        </div>
    );
}

export default EditarPerfilPages;
