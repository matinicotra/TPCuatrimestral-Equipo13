﻿using Dominio;
using Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TPCuatrimestal
{
    public partial class homeChofer : System.Web.UI.Page
    {

        private List<Viaje> viajes = new List<Viaje>();

        private Domicilio domicilioOrigen = new Domicilio();

        private long idChofer = 1;   // !!! cambiar cuando hagamos el login

        protected void Page_Load(object sender, EventArgs e)
        {

            ViajeNegocio viajeNegocio = new ViajeNegocio();
            viajes = viajeNegocio.ViajesClientesChoferes(idChofer, true);

            if (!IsPostBack)
            {
                dgvViajes.DataSource = viajes;
                dgvViajes.DataBind();
                
            }

            CargarMapa(viajes[0]);   // !!! vamos a tener un viaje activo o seria el primero de la lista?
        }

        private void CargarDGVViajes()
        {
            ViajeNegocio viajeNegocio = new ViajeNegocio();
            viajes = viajeNegocio.ViajesClientesChoferes(idChofer, true);
            dgvViajes.DataSource = viajes;
            dgvViajes.DataBind();
        }

        //CARGA EL MAPA CON EL PRIMER VIAJE POR DEFECTO
        protected void CargarMapa(Viaje viaje)
        { 
            string direccion = viaje.Origen.Direccion.ToString().Replace(" ", "+");
            string localidad = viaje.Origen.Localidad.ToString().Replace(" ", "+");
            string provincia = viaje.Origen.Provincia.ToString().Replace(" ", "+");

            string domicilio = direccion + "," + localidad + "," + provincia;

            urlIframe.Attributes.Add("src", "https://www.google.com/maps/embed/v1/place?key=AIzaSyDoBiKY57PiZmKkaMIjWRjSMPZO2i-XJJM&q=" + domicilio);
        }

        protected void btnDetalleViaje_Click(object sender, EventArgs e)
        {
            Response.Redirect("detalleViaje.aspx", false);
        }

        protected void dgvViajes_SelectedIndexChanged(object sender, EventArgs e)
        {
            var numViajeSeleccionado = dgvViajes.SelectedDataKey.Value.ToString();
        }

        protected void btnPagado_Click(object sender, ImageClickEventArgs e)
        {
            ViajeNegocio viajeNegocio = new ViajeNegocio();

            int valorID = int.Parse(((ImageButton)sender).CommandArgument);

            viajeNegocio.PagarDespagarViaje(valorID, true);

            CargarDGVViajes();
        }

        // MAPA ORIGEN
        protected void btnMapa_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton btnImg = (ImageButton)sender;
            long numViaje = long.Parse(btnImg.CommandArgument);
            Viaje viaje = viajes.Find(X => X.NumViaje == numViaje);

            string direccion = viaje.Origen.Direccion.ToString().Replace(" ", "+");
            string localidad = viaje.Origen.Localidad.ToString().Replace(" ", "+");
            string provincia = viaje.Origen.Provincia.ToString().Replace(" ", "+");

            string domicilio = direccion + "," + localidad + "," + provincia;

            urlIframe.Attributes.Add("src", "https://www.google.com/maps/embed/v1/place?key=AIzaSyDoBiKY57PiZmKkaMIjWRjSMPZO2i-XJJM&q=" + domicilio);
        }

        // MAPA DESTINO
        protected void btnMapaDestino_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton btnImg = (ImageButton)sender;
            long numViaje = long.Parse(btnImg.CommandArgument);
            Viaje viaje = viajes.Find(X => X.NumViaje == numViaje);

            string direccion = viaje.Destinos[0].Direccion.ToString().Replace(" ", "+");        // !!! ver lista destinos 
            string localidad = viaje.Destinos[0].Localidad.ToString().Replace(" ", "+");
            string provincia = viaje.Destinos[0].Provincia.ToString().Replace(" ", "+");

            string domicilio = direccion + "," + localidad + "," + provincia;

            urlIframe.Attributes.Add("src", "https://www.google.com/maps/embed/v1/place?key=AIzaSyDoBiKY57PiZmKkaMIjWRjSMPZO2i-XJJM&q=" + domicilio);
        }

        // MENSAJE WHATSAPP
        protected void btnWhatsApp_Click1(object sender, ImageClickEventArgs e)
        {
            ImageButton btnImg = (ImageButton)sender;
            long numViaje = long.Parse(btnImg.CommandArgument);
            Viaje viaje = viajes.Find(X => X.NumViaje == numViaje);

            string telefono = viaje.ClienteViaje.Telefono.ToString();
            Response.Redirect("https://wa.me/" + telefono + "?text=Tu%20transporte%20ha%20llegado!");
        }

        protected void btnFinalizado_Click(object sender, ImageClickEventArgs e)
        {
            ImageButton btnImg = (ImageButton)sender;
            long idViaje = long.Parse(btnImg.CommandArgument);
            ViajeNegocio viajeNegocio = new ViajeNegocio();
            viajeNegocio.FinalizarViaje(idViaje);
        }
    }
}