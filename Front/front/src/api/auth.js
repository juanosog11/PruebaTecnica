// api/auth.js
import axios from "axios";

const API = "https://localhost:8000/api";

// Registrar un nuevo usuario
export const registerUser = async (user) => {
    try {
        const response = await axios.post(`${API}/Acceso/Registrarse`, user, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error al registrar usuario:", error);
        throw error;
    }
};

// Ingresar (login) un usuario
export const Ingresar = async (user) => {
    try {
        const response = await axios.post(`${API}/Acceso/Ingresar`, user, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error al ingresar usuario:", error);
        throw error;
    }
};


export const CerrarSesion = async () => {
    try {
        const response = await axios.post(`${API}/Acceso/CerrarSesion`, {}, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error logging out:", error);
        throw error;
    }
};


// Verificar el token JWT
export const verificarToken = async () => {
    try {
        const response = await axios.get(`${API}/Acceso/VerificarToken`, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error al verificar token:", error.response.data.message);
        throw error;
    }
};

// Obtener todos los usuarios
export const getusuarios = async () => {
    try {
        const response = await axios.get(`${API}/Users`, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error al obtener usuarios:", error);
        throw error;
    }
};

export const getPost = async (postId) => {
    try {
        const response = await axios.get(`${API}/Posts/${postId}`, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error(`Error fetching post ${postId}:`, error);
        throw error;
    }
};

// Obtener posts de los usuarios seguidos
export const getPostsSeguidos = async (usuarioId) => {
    try {
        
        const response = await axios.get(`${API}/Posts/ObtenerPostsSeguidos/${usuarioId}`, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error al obtener posts:", error);
        throw error;
    }
};

export const getUsuarioPosts = async (usuarioId) => {
    try {
        const response = await axios.get(`${API}/Posts/Usuario/${usuarioId}`, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error(`Error fetching posts for usuario ${usuarioId}:`, error);
        throw error;
    }
};

export const updatePost = async (postId, updatedPost) => {
    try {
        const response = await axios.put(`${API}/Posts/${postId}`, updatedPost, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error(`Error updating post ${postId}:`, error);
        throw error;
    }
};

// Eliminar un post por su ID
export const deletePost = async (postId) => {
    try {
        const response = await axios.delete(`${API}/Posts/${postId}`, {
            withCredentials: true,
        });
        return response.data; // Puedes retornar datos adicionales según sea necesario
    } catch (error) {
        console.error("Error al eliminar el post:", error);
        throw error;
    }
};


// Seguir a un usuario
export const seguirUsuario = async (followerId, followeeId) => {
    try {
        const response = await axios.post(`${API}/Follow`, null, {
            params: {
                followerId,
                followeeId,
            },
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error al seguir usuario:", error);
        throw error;
    }
};

export const createPost = async (newPost) => {
    try {
        const { content, UserUsuarioId , usuarioId } = newPost;
        const timestamp = new Date().toISOString(); // Generate the current timestamp
        const response = await axios.post(
            `${API}/Posts`,
            {
                content,
                timestamp,
                UserUsuarioId,
                usuarioId,
                
            },
            { withCredentials: true }
        );
        return response.data;
    } catch (error) {
        console.error("Error creating post:", error);
        throw error;
    }
};

export const getFollowers = async (followeeId) => {
    try {
        const response = await axios.get(`${API}/Follow/followers/${followeeId}`, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error fetching followers:", error);
        throw error;
    }
};

export const getFollowing = async (followerId) => {
    try {
        const response = await axios.get(`${API}/Follow/following/${followerId}`, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error fetching following:", error);
        throw error;
    }
};

export const deleteUser = async (userId) => {
    try {
        const response = await axios.delete(`${API}/Users/${userId}`, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error deleting user:", error);
        throw error;
    }
};

export const updateUser = async (userId, userDetails) => {
    try {
        const response = await axios.patch(`${API}/Users/${userId}`, userDetails, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error updating user:", error);
        throw error;
    }
};

export const dejarDeSeguirUsuario = async (followerId, followeeId) => {
    try {
        const response = await axios.delete(`${API}/Follow/UnfollowUser?followerId=${followerId}&followeeId=${followeeId}`, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error('Error unfollowing user:', error);
        throw error;
    }
};

export const getNotFollowing = async (followerId) => {
    try {
        const response = await axios.get(`${API}/Follow/GetNoFollower/${followerId}`, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error fetching following:", error);
        throw error;
    }
};

export const getUserByUsername = async (username) => {
    try {
        const response = await axios.get(`${API}/Users/ByUserName/${username}`, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error(`Error fetching user ${username}:`, error);
        throw error;
    }
};


export const checkIfFollowing = async (followerId, followeeId) => {
    try {
        const response = await axios.get(`${API}/Follow/checkIfFollowing/${followerId}/${followeeId}`, {
            withCredentials: true,
        });
        return response.data;
    } catch (error) {
        console.error("Error al verificar si sigue usuario:", error);
        throw error;
    }
};

