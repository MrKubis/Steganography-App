from bitstring import BitArray
offset = 4000
message = BitArray(b"Trap utopia")
message_bits = message.bin
print(len(message_bits))
with open("test.png", "rb") as read_file, open("out.png","wb") as write_file:
    i = 0
    byte_index = 0
    while byte := read_file.read(1) :
        b = byte[0]
        if byte_index >= offset and i < len(message_bits):
            bit = int(message_bits[i])
            #Zerowanie ostatniego bitu za pomocą AND i dodawanie za pomocą OR
            b = (b & 0b11111110) | bit
            i+=1
        write_file.write(bytes([b]))
        byte_index+=1
    print("Total bytes written: ", byte_index)